/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Common
{
    /// <summary>
    /// Defines the various supported units for <see cref="ISensor"/> readings.
    /// </summary>
    public enum Unit : short
    {
        Celsius,
        Percent,
        Meter
    }
}
