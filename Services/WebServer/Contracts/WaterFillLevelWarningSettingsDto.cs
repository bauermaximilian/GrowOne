/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class WaterFillLevelWarningSettingsDto
    {
        public bool enabled { get; set; }

        public float minimumLevel { get; set; }

        public float maximumLevel { get; set; }

        public WaterFillLevelWarningSettings ToSettings()
        {
            return new WaterFillLevelWarningSettings()
            {
                Enabled = enabled,
                MinimumLevel = minimumLevel,
                MaximumLevel = maximumLevel
            };
        }

        public static WaterFillLevelWarningSettingsDto? FromSettings(
            WaterFillLevelWarningSettings? settings)
        {
            if (settings == null) return null;
            else return new WaterFillLevelWarningSettingsDto()
            {
                enabled = settings.Enabled,
                minimumLevel = settings.MinimumLevel,
                maximumLevel = settings.MaximumLevel
            };
        }
    }
}
