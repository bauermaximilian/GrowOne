/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;

namespace GrowOne.Hardware.Actuators
{
    /// <summary>
    /// Represents an actuator that can be used to control a specific machine component responsible
    /// for moving or controlling another part or mechanism.
    /// </summary>
    public interface IActuator : IDisposable
    {
        /// <summary>
        /// Sets the current instance to a specific value.
        /// </summary>
        /// <param name="value">The value of the actuator.</param>
        /// <exception cref="ObjectDisposedException">
        /// Is thrown when the current instance was disposed and cannot be used anymore.
        /// </exception>
        void SetValue(bool value);
    }
}
