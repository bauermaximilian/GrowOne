/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Board;

namespace GrowOne.Hardware.Sensors.Distance
{
    internal class FillLevelSensorProvider : Hcsr04SensorProvider
    {
        private readonly double valueMinimum;
        private readonly double valueMaximum;

        public FillLevelSensorProvider(IBoard board, int pinEcho, int pinTrigger,
            double valueMinimum, double valueMaximum) 
            : base(board, pinEcho, pinTrigger)
        {
            this.valueMinimum = valueMinimum;
            this.valueMaximum = valueMaximum;
        }

        public override ISensor OpenSensor(int index)
        {
            return new FillLevelSensor(Board.OpenHcsr04(PinTrigger, PinEcho), 
                valueMinimum, valueMaximum);
        }
    }
}
