/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Sensors.Distance;
using GrowOne.Hardware.Sensors.TemperatureHumidity;
using nanoFramework.Hardware.Esp32;
using System;
using System.Device.Gpio;

namespace GrowOne.Hardware.Board.Esp32
{
    internal class Esp32Board : IBoard
    {
        private readonly object boardLock = new();

        public void SetPwmPin(int pin)
        {
            Configuration.SetPinFunction(pin, DeviceFunction.PWM1);
        }

        public IDht22Wrapper OpenDht22(int pinEcho, int pinTrigger, GpioController? gpioController)
        {
            return new Esp32Dht22HardwareWrapper(pinEcho, pinTrigger, gpioController);
        }

        public IHcsr04SensorWrapper OpenHcsr04(int pinTrigger, int pinEcho)
        {
            return new Hcsr04SensorWrapper(pinTrigger, pinEcho);
        }

        public bool TryGetChannelFromPin(int pin, out int channel)
        {
            channel = pin switch
            {
                36 => 0,
                37 => 1,
                38 => 2,
                39 => 3,
                32 => 4,
                33 => 5,
                34 => 6,
                35 => 7,
                _ => -1
            };
            return channel >= 0;
        }

        public void Invoke(Action action)
        {
            lock (boardLock)
            {
                action();
            }
        }
    }
}
