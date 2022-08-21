/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class AutomaticWateringSettingsDto
    {
        public bool enabled { get; set; }

        public float minimumMoisture { get; set; }

        public int durationSeconds { get; set; }

        public int cooldownSeconds { get; set; }

        public AutomaticWateringSettings ToSettings()
        {
            return new AutomaticWateringSettings()
            {
                Enabled = enabled,
                MinimumMoisture = minimumMoisture,
                DurationSeconds = durationSeconds,
                CooldownSeconds = cooldownSeconds
            };
        }

        public static AutomaticWateringSettingsDto? FromSettings(
            AutomaticWateringSettings? settings)
        {
            if (settings == null) return null;
            else return new AutomaticWateringSettingsDto()
            {
                enabled = settings.Enabled,
                minimumMoisture = settings.MinimumMoisture,
                durationSeconds = settings.DurationSeconds,
                cooldownSeconds = settings.CooldownSeconds
            };
        }
    }
}
