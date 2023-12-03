/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Core.Settings;
using GrowOne.Services.DeviceManager;

namespace GrowOne.Services.MeasurementManager
{
    internal class TemperatureRangeWarner : MeasurementRangeSupervisor
    {
        public const int SignalIntervalSeconds = 30;

        private readonly DeviceManagerService deviceManager;

        public override MeasurementType SupervisedType => MeasurementType.Temperature;

        public TemperatureRangeWarner(TemperatureWarningSettings temperatureWarningSettings,
            DeviceManagerService deviceManager)
            :base(temperatureWarningSettings.MinimumTemperature,
                 temperatureWarningSettings.MaximumTemperature,
                 SignalIntervalSeconds)
        {
            this.deviceManager = deviceManager;
            IsActive = temperatureWarningSettings.Enabled;
        }

        protected override void PerformRangeExceededAction(Measurement measurement)
        {
            deviceManager.PlayNotificationSound(Status.Default);
        }
    }
}
