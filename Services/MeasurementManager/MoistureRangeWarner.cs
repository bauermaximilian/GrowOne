/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using GrowOne.Common.Settings;
using GrowOne.Services.DeviceManager;

namespace GrowOne.Services.MeasurementManager
{
    internal class MoistureRangeWarner : MeasurementRangeSupervisor
    {
        public const int SignalIntervalSeconds = 30;

        private readonly DeviceManagerService deviceManager;

        public override MeasurementType SupervisedType => MeasurementType.Moisture;

        public MoistureRangeWarner(MoistureWarningSettings moistureWarningSettings,
            DeviceManagerService deviceManager)
            :base(moistureWarningSettings.MinimumMoisture, 
                 moistureWarningSettings.MaximumMoisture,
                 SignalIntervalSeconds)
        {
            this.deviceManager = deviceManager;
            IsActive = moistureWarningSettings.Enabled;
        }

        protected override void PerformRangeExceededAction(Measurement measurement)
        {
            deviceManager.PlayNotificationSound(Status.Default);
        }
    }
}
