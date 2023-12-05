/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class MoistureWarningSettingsDto
    {
        public bool on { get; set; }

        public float min { get; set; }

        public float max { get; set; }

        public MoistureWarningSettings ToSettings()
        {
            return new MoistureWarningSettings()
            {
                Enabled = on,
                MinimumMoisture = min,
                MaximumMoisture = max
            };
        }

        public static MoistureWarningSettingsDto? FromSettings(
            MoistureWarningSettings? settings)
        {
            if (settings == null) return null;
            else return new MoistureWarningSettingsDto()
            {
                on = settings.Enabled,
                min = settings.MinimumMoisture,
                max = settings.MaximumMoisture
            };
        }
    }
}
