/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using System;

namespace GrowOne.Services.MeasurementManager
{
    internal interface IMeasurementSupervisor
    {
        MeasurementType SupervisedType { get; }

        void Update(TimeSpan delta, Measurement? measurement);
    }
}
