/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Hardware.Board.Esp32;
using System;

namespace GrowOne.Hardware.Sensors.TemperatureHumidity
{
    internal class Dht22Sensor : ISensor
    {
        public MeasurementType Type => provideTemperature ? 
            MeasurementType.Temperature : MeasurementType.Moisture;

        public Measurement? CurrentMeasurement => provideTemperature ?
            sensorWrapper.Temperature : sensorWrapper.Humidity;

        private readonly IDht22Wrapper sensorWrapper;
        private readonly bool provideTemperature;

        public Dht22Sensor(IDht22Wrapper sensorWrapper, bool provideTemperature)
        {
            this.sensorWrapper = sensorWrapper;
            this.provideTemperature = provideTemperature;
        }

        public void Dispose()
        {
            sensorWrapper.Dispose();
        }

        public void Update(TimeSpan delta)
        {
            sensorWrapper.Update(delta);
        }
    }
}
