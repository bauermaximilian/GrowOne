/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using System;

namespace GrowOne.Hardware.Sensors.Distance
{
    public interface IHcsr04SensorWrapper : IDisposable
    {
        bool IsDisposed { get; }

        Measurement? Distance { get; }

        bool Update(TimeSpan delta);
    }
}
