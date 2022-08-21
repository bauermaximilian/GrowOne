/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Board;
using System;
using System.Device.Gpio;

namespace GrowOne.Hardware.Sensors.TemperatureHumidity
{
    public class Dht22SensorProvider : ISensorProvider
    {
        public int SensorCount => 2;

        private readonly IBoard board;
        private readonly int pinEcho, pinTrigger;
        private readonly GpioController? gpioController;

        private IDht22Wrapper? sensorWrapper;

        public Dht22SensorProvider(IBoard board, int pinEcho, int pinTrigger,
            GpioController? gpioController = null)
        {
            this.board = board;
            this.pinEcho = pinEcho;
            this.pinTrigger = pinTrigger;
            this.gpioController = gpioController;
        }

        public ISensor OpenSensor(int index)
        {
            if (sensorWrapper == null)
            {
                sensorWrapper = board.OpenDht22(pinEcho, pinTrigger, gpioController);
            }

            if (index == 0) return new Dht22Sensor(sensorWrapper, true);
            else if (index == 1) return new Dht22Sensor(sensorWrapper, false);
            else throw new ArgumentOutOfRangeException(nameof(index));
        }
    }
}
