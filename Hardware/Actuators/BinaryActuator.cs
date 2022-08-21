/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Hardware.Board;
using System;
using System.Device.Gpio;

namespace GrowOne.Hardware.Actuators
{
    /// <summary>
    /// Provides a base class from which <see cref="IActuator"/> implementations are derived that
    /// can only accept a boolean value (as in either being active or inactive).
    /// </summary>
    public abstract class BinaryActuator : IActuator
    {
        /// <summary>
        /// Gets the <see cref="System.Device.Gpio.GpioPin"/> instance used by the 
        /// current instance.
        /// </summary>
        protected GpioPin GpioPin { get; }

        /// <summary>
        /// Gets a value indicating whether the current instance was disposed (with the 
        /// <see cref="GpioPin"/>) and can't be used anymore (<c>true</c>) or not (<c>false</c>).
        /// </summary>
        protected bool IsDisposed { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryActuator"/> class.
        /// </summary>
        /// <param name="gpioPin">
        /// The <see cref="System.Device.Gpio.GpioPin"/> instance to be used by the new instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Is thrown when the specified <paramref name="gpioPin"/> is null.
        /// </exception>
        protected BinaryActuator(GpioPin gpioPin)
        {
            GpioPin = gpioPin ??
                throw new ArgumentNullException(nameof(gpioPin));
            SetValue(false);
        }

        public void SetValue(bool value)
        {
            if (IsDisposed)
                throw new ObjectDisposedException();
            
            if (Program.InvertRelaisOutput)
            {
                value = !value;
            }

            GpioPin.Write(value ? PinValue.High : PinValue.Low);
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                GpioPin.Dispose();
                OnDispose();
                IsDisposed = true;
            }
        }

        /// <summary>
        /// Gets called when the current instance is disposed.
        /// </summary>
        protected virtual void OnDispose() { }
    }
}
