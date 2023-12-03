/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Hardware.Sensors
{
    /// <summary>
    /// Provides functionality to instantiate <see cref="ISensor"/> instances of a specific type.
    /// </summary>
    public interface ISensorProvider
    {
        /// <summary>
        /// Gets the amount of sensors that are currently available.
        /// </summary>
        int SensorCount { get; }

        /// <summary>
        /// Creates a new <see cref="ISensor"/> instance for a specific sensor.
        /// </summary>
        /// <param name="index">
        /// The index of the sensor inside the current instance (see <see cref="SensorCount"/>).
        /// </param>
        /// <returns>
        /// A new <see cref="ISensor"/> instance.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Is thrown when the specified <paramref name="index"/> exceeds the current 
        /// <see cref="SensorCount"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Is thrown when the sensor couldn't be accessed.
        /// </exception>
        ISensor OpenSensor(int index);
    }
}
