/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Board;
using System;
using System.Device.Adc;

namespace GrowOne.Hardware.Sensors
{
    public abstract class AnalogSensorProvider : ISensorProvider
    {
        public int SensorCount => pinIndicies.Length;

        private readonly AdcController adcController;
        private readonly int[] pinIndicies;
        private readonly IBoard board;

        public AnalogSensorProvider(int[] pins, IBoard board, AdcController? adcController = null)
        {
            this.adcController = adcController ?? new AdcController();
            
            this.adcController.ChannelMode = AdcChannelMode.SingleEnded;
            pinIndicies = pins ??
                throw new ArgumentNullException(nameof(pins));
            this.board = board;
        }

        public ISensor OpenSensor(int index)
        {
            if (index < 0 || index >= pinIndicies.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            if (!board.TryGetChannelFromPin(pinIndicies[index], out int channelIndex))
                throw new Exception($"The pin #{pinIndicies[index]} can't be used for an " +
                    $"analog sensor - try pins 32 to 39 instead.");

            AdcChannel channel;
            try
            {
                channel = adcController.OpenChannel(channelIndex);
                // Perform a test read to make fail here if something is wrong.
                _ = channel.ReadRatio();
                return CreateSensor(channel);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException($"The sensor #{index} " +
                    $"(channel #{channelIndex}) couldn't be accessed.", exc);
            }
        }

        protected abstract ISensor CreateSensor(AdcChannel channel);
    }
}
