/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Hardware.Sensors.Distance;
using Iot.Device.Hcsr04.Esp32;
using System;

namespace GrowOne.Hardware.Board.Esp32
{
    internal class Hcsr04SensorWrapper : IHcsr04SensorWrapper
    {
        public bool IsDisposed { get; private set; }

        public Measurement? Distance { get; private set; }

        private readonly Hcsr04 sensor;

        public Hcsr04SensorWrapper(int pinTrigger, int pinEcho)
        {
            sensor = new Hcsr04(pinTrigger, pinEcho);
        }

        public bool Update(TimeSpan delta)
        {
            if (!IsDisposed)
            {
                try
                {
                    if (sensor.TryGetDistance(out var result))
                    {
                        Distance = new Measurement(result.Meters, MeasurementType.Distance);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
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

        public void Dispose()
        {
            if (!IsDisposed)
            {
                sensor.Dispose();
                IsDisposed = true;
            }
        }
    }
}
