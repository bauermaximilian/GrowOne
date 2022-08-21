/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using System;

namespace GrowOne.Hardware.Sensors.TemperatureHumidity
{
    public interface IDht22Wrapper : IDisposable
    {
        bool IsDisposed { get; }

        Measurement? Temperature { get; }

        Measurement? Humidity { get; }

        bool Update(TimeSpan delta);
    }
}
