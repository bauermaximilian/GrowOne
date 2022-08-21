/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Common.Settings
{
    public class AutomaticWateringSettings
    {
        public bool Enabled { get; set; }

        public float MinimumMoisture { get; set; }

        public int DurationSeconds { get; set; }

        public int CooldownSeconds { get; set; }

        public AutomaticWateringSettings Clone()
        {
            return new AutomaticWateringSettings()
            {
                Enabled = Enabled,
                MinimumMoisture = MinimumMoisture,
                DurationSeconds = DurationSeconds,
                CooldownSeconds = CooldownSeconds
            };
        }
    }
}
