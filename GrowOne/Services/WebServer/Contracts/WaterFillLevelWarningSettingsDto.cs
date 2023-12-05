/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class WaterFillLevelWarningSettingsDto
    {
        public bool on { get; set; }

        public float min { get; set; }

        public float max { get; set; }

        public WaterFillLevelWarningSettings ToSettings()
        {
            return new WaterFillLevelWarningSettings()
            {
                Enabled = on,
                MinimumLevel = min,
                MaximumLevel = max
            };
        }

        public static WaterFillLevelWarningSettingsDto? FromSettings(
            WaterFillLevelWarningSettings? settings)
        {
            if (settings == null) return null;
            else return new WaterFillLevelWarningSettingsDto()
            {
                on = settings.Enabled,
                min = settings.MinimumLevel,
                max = settings.MaximumLevel
            };
        }
    }
}
