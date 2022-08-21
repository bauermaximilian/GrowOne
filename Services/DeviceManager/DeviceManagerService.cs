/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using GrowOne.Common.Settings;
using GrowOne.Hardware.Actuators;
using GrowOne.Hardware.Actuators.Relais;
using GrowOne.Hardware.Board;
using GrowOne.Hardware.Sensors;
using GrowOne.Hardware.Sensors.Distance;
using GrowOne.Hardware.Sensors.SoilMoisture;
using GrowOne.Hardware.Sensors.TemperatureHumidity;
using GrowOne.Hardware.Signalers;
using GrowOne.Hardware.Signalers.Buzzer;
using System;
using System.Device.Adc;
using System.Device.Gpio;
using System.Threading;

namespace GrowOne.Services.DeviceManager
{
    internal class DeviceManagerService : BackgroundService
    {
        private DateTime lastWateringEnd = DateTime.MinValue;
        private Status lastNotificationSoundStatus = Status.None;

        private readonly GpioController gpioController = new();
        private readonly AdcController adcController = new();

        private readonly HardwareSettings hardwareSettings;
        private readonly ApplicationService applicationService;
        private readonly IBoard board;
        private readonly IActuatorProvider relaisActuatorProvider;

        public TimeSpan LastWatering 
        {
            get
            {
                TimeSpan lastWatering = DateTime.UtcNow - lastWateringEnd;
                if (lastWatering.Ticks > 0) return lastWatering;
                else return TimeSpan.Zero;
            }
        }

        public string LastWateringFormatted
        {
            get
            {
                TimeSpan lastWatering = LastWatering;
                if (lastWatering.TotalHours < 99)
                {
                    return $"{lastWatering.Hours}:{lastWatering.Minutes}h";
                }
                else
                {
                    return "\u221e";
                }
            }
        }

        public DeviceManagerService(HardwareSettings hardwareSettings,
            ApplicationService applicationService, IBoard board)
        {
            this.hardwareSettings = hardwareSettings;
            this.applicationService = applicationService;
            this.board = board;
            relaisActuatorProvider = new RelaisActuatorProvider(
                new int[] { hardwareSettings.IrrigatorSwitchPin,
                    hardwareSettings.SensorSwitchPin }, gpioController);
        }

        private IStatusSignaler? TryOpenBuzzer()
        {
            try
            {
                return (IStatusSignaler)(new BuzzerSignalerProvider(board,
                    new int[] { hardwareSettings.BuzzerPin }).OpenSignaler(0));
            }
            catch (Exception exc)
            {
                Log.Error($"Couldn't open buzzer at pin #{hardwareSettings.BuzzerPin}.", exc);
                return null;
            }
        }

        private IActuator? TryOpenIrrigatorSwitch()
        {
            try
            {
                return relaisActuatorProvider.GetActuator(0);
            }
            catch (Exception exc)
            {
                Log.Error($"Couldn't open relais at pin #{hardwareSettings.IrrigatorSwitchPin}.", exc);
                return null;
            }
        }

        private IActuator? TryOpenSensorSwitch()
        {
            try
            {
                return relaisActuatorProvider.GetActuator(1);
            }
            catch (Exception exc)
            {
                Log.Error($"Couldn't open relais at pin #{hardwareSettings.SensorSwitchPin}.", exc);
                return null;
            }
        }

        protected override void Run(CancellationToken token)
        {
            IStatusSignaler? buzzer = TryOpenBuzzer();
            IActuator? irrigatorSwitch = TryOpenIrrigatorSwitch();
            IActuator? sensorSwitch = TryOpenSensorSwitch();

            if (sensorSwitch != null)
            {
                sensorSwitch.SetValue(true);
            }

            bool isCurrentlyWatering = false;
            Status lastExecutedNotificationStatus = Status.None;
            while (!token.IsCancellationRequested)
            {
                bool shouldBeWatering = DateTime.UtcNow < lastWateringEnd;
                if (shouldBeWatering != isCurrentlyWatering)
                {
                    isCurrentlyWatering = shouldBeWatering;
                    irrigatorSwitch?.SetValue(shouldBeWatering);
                }

                if (lastNotificationSoundStatus != lastExecutedNotificationStatus)
                {
                    lastExecutedNotificationStatus = lastNotificationSoundStatus;
                    if (buzzer != null)
                    {
                        buzzer.SendSignal(lastExecutedNotificationStatus);
                        lastExecutedNotificationStatus = lastNotificationSoundStatus = 
                            Status.None;
                    }
                }

                Thread.Sleep(250);
            }

            if (sensorSwitch != null)
            {
                sensorSwitch.SetValue(false);
            }
        }

        public void PlayNotificationSound(Status status)
        {
            lastNotificationSoundStatus = status;
        }

        public void PerformWatering(TimeSpan duration)
        {
            lastWateringEnd = DateTime.UtcNow + duration;
        }

        public void RestartDevice()
        {
            applicationService.Stop();
        }

        public ISensorProvider[] GetSensorProviders()
        {
            return new ISensorProvider[]
            {
                new Dht22SensorProvider(board, hardwareSettings.Dht22EchoPin, 
                    hardwareSettings.Dht22TriggerPin, gpioController),
                new SoilMoistureSensorProvider(
                    new int[] { hardwareSettings.MoistureSensorPin }, board, adcController) ,
                new FillLevelSensorProvider(
                    board, hardwareSettings.Hcsr04EchoPin, hardwareSettings.Hcsr04TriggerPin,
                    hardwareSettings.Hcsr04MinimumValueCm / 100.0, 
                    hardwareSettings.Hcsr04MaximumValueCm / 100.0)
            };
        }
    }
}
