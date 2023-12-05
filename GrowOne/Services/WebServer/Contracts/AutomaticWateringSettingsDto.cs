/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class AutomaticWateringSettingsDto
    {
        public bool on { get; set; }

        public float min { get; set; }

        public int dur { get; set; }

        public int cd { get; set; }

        public AutomaticWateringSettings ToSettings()
        {
            return new AutomaticWateringSettings()
            {
                Enabled = on,
                MinimumMoisture = min,
                DurationSeconds = dur,
                CooldownSeconds = cd
            };
        }

        public static AutomaticWateringSettingsDto? FromSettings(
            AutomaticWateringSettings? settings)
        {
            if (settings == null) return null;
            else return new AutomaticWateringSettingsDto()
            {
                on = settings.Enabled,
                min = settings.MinimumMoisture,
                dur = settings.DurationSeconds,
                cd = settings.CooldownSeconds
            };
        }
    }
}
