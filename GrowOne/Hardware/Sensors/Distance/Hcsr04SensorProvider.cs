/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Board;

namespace GrowOne.Hardware.Sensors.Distance
{
    internal class Hcsr04SensorProvider : ISensorProvider
    {
        protected IBoard Board { get; }

        protected int PinEcho { get; }

        protected int PinTrigger { get; }

        public int SensorCount => 1;

        public Hcsr04SensorProvider(IBoard board, int pinEcho, int pinTrigger)
        {
            Board = board;
            PinEcho = pinEcho;
            PinTrigger = pinTrigger;
        }

        public virtual ISensor OpenSensor(int index)
        {
            return new Hcsr04Sensor(Board.OpenHcsr04(PinTrigger, PinEcho));
        }
    }
}
