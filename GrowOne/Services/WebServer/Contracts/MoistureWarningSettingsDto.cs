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
        public bool enabled { get; set; }

        public float minimumMoisture { get; set; }

        public float maximumMoisture { get; set; }

        public MoistureWarningSettings ToSettings()
        {
            return new MoistureWarningSettings()
            {
                Enabled = enabled,
                MinimumMoisture = minimumMoisture,
                MaximumMoisture = maximumMoisture
            };
        }

        public static MoistureWarningSettingsDto? FromSettings(
            MoistureWarningSettings? settings)
        {
            if (settings == null) return null;
            else return new MoistureWarningSettingsDto()
            {
                enabled = settings.Enabled,
                minimumMoisture = settings.MinimumMoisture,
                maximumMoisture = settings.MaximumMoisture
            };
        }
    }
}
