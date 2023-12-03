/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Core.Settings
{
    public class MoistureWarningSettings
    {
        public bool Enabled { get; set; }

        public float MinimumMoisture { get; set; }

        public float MaximumMoisture { get; set; }

        public MoistureWarningSettings Clone()
        {
            return new MoistureWarningSettings()
            {
                Enabled = Enabled,
                MinimumMoisture = MinimumMoisture,
                MaximumMoisture = MaximumMoisture
            };
        }
    }
}
