/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System.Device.Gpio;

namespace GrowOne.Hardware.Actuators.Relais
{
    class RelaisActuator : BinaryActuator
    {
        public RelaisActuator(GpioPin gpioPin) : base(gpioPin) { }
    }
}
