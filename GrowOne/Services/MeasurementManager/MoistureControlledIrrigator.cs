/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Core.Settings;
using GrowOne.Services.DeviceManager;
using System;

namespace GrowOne.Services.MeasurementManager
{
    internal class MoistureControlledIrrigator : MeasurementRangeSupervisor
    {
        public override MeasurementType SupervisedType => MeasurementType.Moisture;

        private readonly int irrigationDurationSeconds;
        private readonly DeviceManagerService deviceManager;

        public MoistureControlledIrrigator(AutomaticWateringSettings automaticWateringSettings,
            DeviceManagerService deviceManager)
            : base(automaticWateringSettings.MinimumMoisture, double.MaxValue, 
                  automaticWateringSettings.CooldownSeconds)
        {
            irrigationDurationSeconds = automaticWateringSettings.DurationSeconds;
            this.deviceManager = deviceManager;
            IsActive = automaticWateringSettings.Enabled;
        }

        protected override void PerformRangeExceededAction(Measurement measurement)
        {
            deviceManager.PerformWatering(TimeSpan.FromSeconds(irrigationDurationSeconds));
        }
    }
}
