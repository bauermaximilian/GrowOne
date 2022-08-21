/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Common.Settings
{
    public class WaterFillLevelWarningSettings
    {
        public bool Enabled { get; set; }

        public float MinimumLevel { get; set; }

        public float MaximumLevel { get; set; }

        public WaterFillLevelWarningSettings Clone()
        {
            return new WaterFillLevelWarningSettings()
            {
                Enabled = Enabled,
                MinimumLevel = MinimumLevel,
                MaximumLevel = MaximumLevel
            };
        }
    }
}
