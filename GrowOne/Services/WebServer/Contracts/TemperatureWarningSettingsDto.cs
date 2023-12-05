/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class TemperatureWarningSettingsDto
    {
        public bool on { get; set; }

        public float min { get; set; }

        public float max { get; set; }

        public TemperatureWarningSettings ToSettings()
        {
            return new TemperatureWarningSettings()
            {
                Enabled = on,
                MinimumTemperature = min,
                MaximumTemperature = max
            };
        }

        public static TemperatureWarningSettingsDto? FromSettings(
            TemperatureWarningSettings? settings)
        {
            if (settings == null) return null;
            else return new TemperatureWarningSettingsDto()
            {
                on = settings.Enabled,
                min = settings.MinimumTemperature,
                max = settings.MaximumTemperature
            };
        }
    }
}
