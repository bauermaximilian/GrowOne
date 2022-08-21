/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Board;
using System.Device.Adc;

namespace GrowOne.Hardware.Sensors.SoilMoisture
{
    public class SoilMoistureSensorProvider : AnalogSensorProvider
    {
        public SoilMoistureSensorProvider(int[] pins, IBoard board, AdcController? adcController = null)
            : base(pins, board, adcController)
        {
        }

        protected override ISensor CreateSensor(AdcChannel channel)
        {
            return new SoilMoistureSensor(channel);
        }
    }
}
