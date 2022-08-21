/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;
using System.Device.Gpio;

namespace GrowOne.Hardware.Actuators.Relais
{
    public class RelaisActuatorProvider : IActuatorProvider
    {
        public int ActuatorCount => pinIndicies.Length;

        private readonly GpioController gpioController;
        private readonly int[] pinIndicies;

        public RelaisActuatorProvider(int[] pins, GpioController? gpioController = null)
        {
            this.gpioController = gpioController ?? new GpioController();
            pinIndicies = pins ?? throw new ArgumentNullException(nameof(pins));
        }

        public IActuator GetActuator(int index)
        {
            if (index < 0 || index >= pinIndicies.Length)
                throw new ArgumentOutOfRangeException(nameof(index));

            var pinIndex = pinIndicies[index];
            GpioPin pin;
            try
            {
                pin = gpioController.OpenPin(pinIndex, PinMode.Output);
                return new RelaisActuator(pin);
            }
            catch (Exception exc)
            {
                throw new InvalidOperationException($"The relais output #{index} " +
                    $"(pin #{pinIndex}) couldn't be accessed.", exc);
            }
        }
    }
}
