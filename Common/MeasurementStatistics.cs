/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;

namespace GrowOne.Common
{
    public class MeasurementStatistics
    {
        public MeasurementType MeasurementType { get; }

        public Measurement CurrentValue { get; }

        public Measurement MaximumValue { get; }

        public Measurement MinimumValue { get; }

        public MeasurementStatistics(Measurement measurement) 
            :this(measurement.Type, measurement.Value, measurement.Value, measurement.Value)
        {
        }

        private MeasurementStatistics(MeasurementType type, double currentValue,
            double maximumValue, double minimumValue)
        {
            MeasurementType = type;
            CurrentValue = new Measurement(currentValue, type);
            MaximumValue = new Measurement(maximumValue, type);
            MinimumValue = new Measurement(minimumValue, type);
        }

        public MeasurementStatistics WithMeasurement(Measurement measurement)
        {
            return new MeasurementStatistics(measurement.Type, measurement.Value,
                Math.Max(MaximumValue.Value, measurement.Value),
                Math.Min(MinimumValue.Value, measurement.Value));
        }
    }
}
