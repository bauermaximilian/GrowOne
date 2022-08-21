/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using System;

namespace GrowOne.Hardware.Sensors.Distance
{
    internal class FillLevelSensor : Hcsr04Sensor
    {
        private readonly double valueMinimum;
        private readonly double valueRange;
        private Measurement? currentMeasurement;

        public override MeasurementType Type => MeasurementType.FillLevel;

        public override Measurement? CurrentMeasurement => currentMeasurement;

        public FillLevelSensor(IHcsr04SensorWrapper sensorWrapper, double valueMinimum,
            double valueMaximum) : base(sensorWrapper)
        {
            valueMinimum = Math.Min(valueMinimum, valueMaximum);
            valueMaximum = Math.Max(valueMinimum, valueMaximum);

            this.valueMinimum = valueMinimum;
            valueRange = valueMaximum - valueMinimum;
        }

        public override void Update(TimeSpan delta)
        {
            base.Update(delta);

            if (valueRange > 0)
            {
                double distance = (BaseSensor.Distance != null ? BaseSensor.Distance.Value : 0);
                double fillLevel = 1 - ((distance - valueMinimum) / valueRange);
                double fillLevelPercent = Math.Min(1, Math.Max(0, fillLevel)) * 100;
                currentMeasurement = new Measurement(fillLevelPercent, Type);
            }
        }
    }
}
