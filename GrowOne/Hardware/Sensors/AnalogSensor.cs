/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Core.Exceptions;
using System;
using System.Device.Adc;

namespace GrowOne.Hardware.Sensors
{
    public abstract class AnalogSensor : ISensor
    {
        private readonly AdcChannel sensorChannel;
        private bool isDisposed = false;

        private double lastMeasurement;
        private bool firstMeasurementTaken;

        /// <summary>
        /// Gets the maximum expectable noise amplitude for the current channel, which 
        /// defines the maximum expectable amount of noise (as ratio) in either "direction" that 
        /// the returned measurement deviates from the actual value.
        /// </summary>
        /// <remarks>
        /// If the connected component is a rotary switch at exactly 50% position, the channel
        /// should ideally return a value of 0.5. Depending on the hardware, the readings can
        /// contain some noise - and, for example, return something between 0.4 and 0.6.
        /// In this case, the noise amplitude would be 0.1.
        /// The higher this value, the longer it will take for the <see cref="CurrentMeasurement"/>
        /// to "change" to the actual value, but lower the deviations/noise from the actual
        /// value will be.
        /// </remarks>
        protected abstract double ChannelNoiseAmplitude { get; }

        public abstract MeasurementType Type { get; }

        public Measurement? CurrentMeasurement { get; private set; }

        public AnalogSensor(AdcChannel sensorChannel)
        {
            this.sensorChannel = sensorChannel;
        }

        public void Update(TimeSpan delta)
        {
            if (!isDisposed)
            {
                double currentMeasurement = sensorChannel.ReadRatio();

                if (firstMeasurementTaken)
                {
                    double measurementDelta = currentMeasurement - lastMeasurement;
                    double measurementWeight = 0.01 / (ChannelNoiseAmplitude * 2);
                    lastMeasurement += measurementDelta * delta.TotalSeconds * measurementWeight;
                }
                else
                {
                    firstMeasurementTaken = true;
                    lastMeasurement = currentMeasurement;
                }

                CurrentMeasurement = CalculateMeasurement(lastMeasurement);
                if (CurrentMeasurement.Type != Type)
                    throw new InternalException("The type of the calculated " +
                        "measurement doesn't match with the type of the sensor.");
            }
            else throw new ObjectDisposedException();
        }

        protected abstract Measurement CalculateMeasurement(double ratio);

        public void Dispose()
        {
            if (!isDisposed)
            {
                sensorChannel.Dispose();
                isDisposed = true;
            }
        }
    }
}
