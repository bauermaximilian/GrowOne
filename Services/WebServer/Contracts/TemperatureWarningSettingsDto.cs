/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class TemperatureWarningSettingsDto
    {
        public bool enabled { get; set; }

        public float minimumTemperature { get; set; }

        public float maximumTemperature { get; set; }

        public TemperatureWarningSettings ToSettings()
        {
            return new TemperatureWarningSettings()
            {
                Enabled = enabled,
                MinimumTemperature = minimumTemperature,
                MaximumTemperature = maximumTemperature
            };
        }

        public static TemperatureWarningSettingsDto? FromSettings(
            TemperatureWarningSettings? settings)
        {
            if (settings == null) return null;
            else return new TemperatureWarningSettingsDto()
            {
                enabled = settings.Enabled,
                minimumTemperature = settings.MinimumTemperature,
                maximumTemperature = settings.MaximumTemperature
            };
        }
    }
}
