/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Core.Settings
{
    public class ApplicationSettings
    {
        public AutomaticWateringSettings? AutomaticWateringSettings { get; set; }

        public MoistureWarningSettings? MoistureWarningSettings { get; set; }

        public TemperatureWarningSettings? TemperatureWarningSettings { get; set; }

        public WaterFillLevelWarningSettings? WaterFillLevelWarningSettings { get; set; }

        public HardwareSettings? HardwareSettings { get; set; }

        public static ApplicationSettings CreateDefault()
        {
            return new ApplicationSettings()
            {
                AutomaticWateringSettings = new AutomaticWateringSettings()
                {
                    Enabled = false,
                    CooldownSeconds = 900,
                    DurationSeconds = 10,
                    MinimumMoisture = 0.5f
                },
                MoistureWarningSettings = new MoistureWarningSettings()
                {
                    Enabled = false,
                    MaximumMoisture = 1,
                    MinimumMoisture = 0.4f
                },
                TemperatureWarningSettings = new TemperatureWarningSettings()
                {
                    Enabled = false,
                    MaximumTemperature = 28,
                    MinimumTemperature = 17
                },
                WaterFillLevelWarningSettings = new WaterFillLevelWarningSettings()
                {
                    Enabled = false,
                    MinimumLevel = 0.25f,
                    MaximumLevel = 1
                },
                HardwareSettings = new HardwareSettings()
            };
        }

        public ApplicationSettings Clone()
        {
            return new ApplicationSettings()
            {
                AutomaticWateringSettings = AutomaticWateringSettings?.Clone(),
                MoistureWarningSettings = MoistureWarningSettings?.Clone(),
                TemperatureWarningSettings = TemperatureWarningSettings?.Clone(),
                WaterFillLevelWarningSettings = WaterFillLevelWarningSettings?.Clone(),
                HardwareSettings = HardwareSettings?.Clone()
            };
        }
    }
}
