/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
using System;

namespace GrowOne.Common.Exceptions
{
    /// <summary>
    /// Represents an <see cref="Exception"/> for unexpected internal errors that shouldn't have
    /// happened and most likely are caused by a faulty implementation or incorrect assertions.
    /// </summary>
    public class InternalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalException"/> class.
        /// </summary>
        public InternalException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message that describes the error.
        /// </param>
        public InternalException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalException"/> class.
        /// </summary>
        /// <param name="message">
        /// The error message that explains the reason for the exception.
        /// </param>
        /// <param name="innerException">
        /// The exception that is the cause for the current exception, or null.
        /// </param>
        public InternalException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
