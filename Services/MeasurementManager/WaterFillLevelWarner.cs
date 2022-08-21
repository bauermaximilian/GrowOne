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
    internal class WaterFillLevelWarner : MeasurementRangeSupervisor
    {
        public const int SignalIntervalSeconds = 30;

        private readonly DeviceManagerService deviceManager;

        public override MeasurementType SupervisedType => MeasurementType.FillLevel;

        public WaterFillLevelWarner(WaterFillLevelWarningSettings waterFillLevelWarningSettings,
            DeviceManagerService deviceManager)
            :base(waterFillLevelWarningSettings.MinimumLevel,
                 waterFillLevelWarningSettings.MaximumLevel,
                 SignalIntervalSeconds)
        {
            this.deviceManager = deviceManager;
            IsActive = waterFillLevelWarningSettings.Enabled;
        }

        protected override void PerformRangeExceededAction(Measurement measurement)
        {
            deviceManager.PlayNotificationSound(Status.Default);
        }
    }
}
