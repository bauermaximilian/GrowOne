/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;

namespace GrowOne.Common
{
    /// <summary>
    /// Provides extension methods for the <see cref="Unit"/> enum.
    /// </summary>
    public static class UnitExtensions
    {
        /// <summary>
        /// Converts a specific <see cref="Unit"/> into its unit symbol, which can be appended
        /// to a value (in that specific unit).
        /// </summary>
        /// <param name="unit">
        /// The <see cref="Unit"/>.
        /// </param>
        /// <returns>
        /// A new <see cref="string"/> instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Is thrown when the specified <paramref name="unit"/> is invalid.
        /// </exception>
        public static string ToUnitSymbol(this Unit unit)
        {
            switch (unit)
            {
                case Unit.Celsius: return "°C";
                case Unit.Percent: return "%";
                default: throw new ArgumentException("The specified unit is invalid.");
            }
        }
    }
}
