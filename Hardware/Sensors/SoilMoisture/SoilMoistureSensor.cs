/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using System;
using System.Device.Adc;

namespace GrowOne.Hardware.Sensors.SoilMoisture
{
    internal class SoilMoistureSensor : AnalogSensor
    {
        private const double RatioMinimum = 0.16;
        private const double RatioMaximum = 0.65;
        private const double RatioRange = RatioMaximum - RatioMinimum;

        public override MeasurementType Type => MeasurementType.Moisture;

        protected override double ChannelNoiseAmplitude => 0.05;

        public SoilMoistureSensor(AdcChannel sensorChannel) : base(sensorChannel)
        {
        }

        protected override Measurement CalculateMeasurement(double ratio)
        {
            double rangedRatio = Math.Max(0, Math.Min(1, 
                ((1 - ratio - RatioMinimum) / RatioRange)));
            return new Measurement(rangedRatio * 100, MeasurementType.Moisture);
        }
    }
}
