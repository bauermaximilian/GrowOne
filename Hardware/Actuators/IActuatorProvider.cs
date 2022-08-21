/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Hardware.Actuators
{
    /// <summary>
    /// Provides functionality to instantiate <see cref="IActuator"/> instances of a specific type.
    /// </summary>
    public interface IActuatorProvider
    {
        /// <summary>
        /// Gets the amount of actuators that are currently available.
        /// </summary>
        int ActuatorCount { get; }

        /// <summary>
        /// Creates a new <see cref="IActuator"/> instance for a specific actuator.
        /// </summary>
        /// <param name="index">
        /// The index of the actuator inside the current instance (see <see cref="ActuatorCount"/>).
        /// </param>
        /// <returns>
        /// A new <see cref="IActuator"/> instance.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Is thrown when the specified <paramref name="index"/> exceeds the current 
        /// <see cref="ActuatorCount"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Is thrown when the actuator couldn't be accessed.
        /// </exception>
        IActuator GetActuator(int index);
    }
}
