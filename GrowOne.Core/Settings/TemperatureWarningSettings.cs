/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Core.Settings
{
    public class TemperatureWarningSettings
    {
        public bool Enabled { get; set; }

        public float MinimumTemperature { get; set; }

        public float MaximumTemperature { get; set; }

        public TemperatureWarningSettings Clone()
        {
            return new TemperatureWarningSettings()
            {
                Enabled = Enabled,
                MinimumTemperature = MinimumTemperature,
                MaximumTemperature = MaximumTemperature
            };
        }
    }
}
