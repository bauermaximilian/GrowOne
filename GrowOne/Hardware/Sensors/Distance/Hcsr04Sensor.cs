/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using System;

namespace GrowOne.Hardware.Sensors.Distance
{
    internal class Hcsr04Sensor : ISensor
    {
        protected IHcsr04SensorWrapper BaseSensor { get; }

        public virtual MeasurementType Type => MeasurementType.Distance;

        public virtual Measurement? CurrentMeasurement => BaseSensor.Distance;

        public Hcsr04Sensor(IHcsr04SensorWrapper sensorWrapper)
        {
            BaseSensor = sensorWrapper;
        }

        public void Dispose()
        {
            BaseSensor.Dispose();
        }

        public virtual void Update(TimeSpan delta)
        {
            BaseSensor.Update(delta);
        }
    }
}
