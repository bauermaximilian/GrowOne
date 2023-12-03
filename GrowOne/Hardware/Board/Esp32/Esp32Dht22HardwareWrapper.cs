/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Hardware.Sensors.TemperatureHumidity;
using Iot.Device.DHTxx.Esp32;
using System;
using System.Device.Gpio;

namespace GrowOne.Hardware.Board.Esp32
{
    internal class Esp32Dht22HardwareWrapper : IDht22Wrapper
    {
        private readonly TimeSpan timeUntilUpdate = TimeSpan.FromSeconds(2);

        private readonly object sensorLock = new();
        private readonly Dht22 sensor;

        private TimeSpan timeSinceLastUpdate = TimeSpan.Zero;

        public bool IsDisposed { get; private set; }

        public Measurement? Temperature { get; private set; }

        public Measurement? Humidity { get; private set; }

        public Esp32Dht22HardwareWrapper(int pinEcho, int pinTrigger, GpioController? gpioController)
        {
            sensor = new Dht22(pinEcho, pinTrigger,
                PinNumberingScheme.Logical, gpioController, true);
        }

        public bool Update(TimeSpan delta)
        {
            lock (sensorLock)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException();
                }

                timeSinceLastUpdate += delta;
                if (timeSinceLastUpdate > timeUntilUpdate)
                {
                    var temperature = sensor.Temperature;
                    var humidity = sensor.Humidity;
                    if (sensor.IsLastReadSuccessful)
                    {
                        try
                        {
                            Temperature = new Measurement(temperature.DegreesCelsius,
                                MeasurementType.Temperature);
                            Humidity = new Measurement(humidity.Percent,
                                MeasurementType.Humidity);
                            timeSinceLastUpdate = TimeSpan.Zero;
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public void Dispose()
        {
            lock (sensorLock)
            {
                if (!IsDisposed)
                {
                    sensor.Dispose();
                    IsDisposed = true;
                }
            }
        }
    }
}
