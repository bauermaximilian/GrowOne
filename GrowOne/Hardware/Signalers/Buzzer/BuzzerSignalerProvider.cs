/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Board;
using System;

namespace GrowOne.Hardware.Signalers.Buzzer
{
    public class BuzzerSignalerProvider : ISignalerProvider
    {
        public int SignalerCount => pinIndicies.Length;

        private readonly IBoard board;
        private readonly int[] pinIndicies;

        public BuzzerSignalerProvider(IBoard board, int[] pins)
        {
            this.board = board;
            pinIndicies = pins ?? throw new ArgumentNullException(nameof(pins));
        }

        public ISignaler OpenSignaler(int index)
        {
            if (index < 0 || index >= pinIndicies.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            var pinIndex = pinIndicies[index];
            try
            {
                return new BuzzerSignaler(board, pinIndex);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException($"The buzzer #{index} " +
                    $"(pin #{pinIndex}) couldn't be accessed.", exc);
            }
        }
    }
}
