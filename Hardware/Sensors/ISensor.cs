/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using System;

namespace GrowOne.Hardware.Sensors
{
    /// <summary>
    /// Represents a sensor that can be used to retrieve a single value in a specific unit.
    /// </summary>
    public interface ISensor : IDisposable
    {
        /// <summary>
        /// Gets the type of the measurements that can be taken by this instance.
        /// </summary>
        MeasurementType Type { get; }

        /// <summary>
        /// Gets the current measurement, or null if the <see cref="Update(TimeSpan)"/>
        /// method wasn't called yet (or couldn't retrieve a valid <see cref="Measurement"/> yet).
        /// </summary>
        Measurement? CurrentMeasurement { get; }

        /// <summary>
        /// Updates the <see cref="CurrentMeasurement"/>, using the current reading of
        /// the sensor.
        /// </summary>
        /// <param name="delta">
        /// The time elapsed since the last update.
        /// </param>
        /// <remarks>
        /// The <see cref="CurrentMeasurement"/> may be calculated as a (denoised) average of the 
        /// last readings. For optimal results, this method should be called a few times per
        /// second and any calibration values of the instance should be set before, if applicable.
        /// </remarks>
        /// <exception cref="ObjectDisposedException">
        /// Is thrown when this instance was disposed before and can't be used anymore.
        /// </exception>
        void Update(TimeSpan delta);
    }
}
