/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Hardware.Signalers
{
    /// <summary>
    /// Provides functionality to instantiate <see cref="ISignaler"/> instances of a specific type.
    /// </summary>
    public interface ISignalerProvider
    {
        /// <summary>
        /// Gets the amount of signalers that are currently available.
        /// </summary>
        int SignalerCount { get; }

        /// <summary>
        /// Creates a new <see cref="ISignaler"/> instance of a specific signaler.
        /// </summary>
        /// <param name="index">
        /// The index of the signaler inside the current instance 
        /// (see <see cref="SignalerCount"/>).
        /// </param>
        /// <returns>
        /// A new <see cref="ISignaler"/> instance.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// Is thrown when the specified <paramref name="index"/> exceeds the current 
        /// <see cref="SignalerCount"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Is thrown when the signlaer couldn't be accessed.
        /// </exception>
        ISignaler OpenSignaler(int index);
    }
}
