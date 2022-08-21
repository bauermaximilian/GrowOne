/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class ApplicationSettingsDto
    {
        public AutomaticWateringSettingsDto? automaticWateringSettings { get; set; }

        public MoistureWarningSettingsDto? moistureWarningSettings { get; set; }

        public TemperatureWarningSettingsDto? temperatureWarningSettings { get; set; }

        public WaterFillLevelWarningSettingsDto? waterFillLevelWarningSettings { get; set; }

        public HardwareSettingsDto? hardwareSettings { get; set; }

        public ApplicationSettings ToSettings()
        {
            return new ApplicationSettings()
            {
                AutomaticWateringSettings = automaticWateringSettings?.ToSettings(),
                MoistureWarningSettings = moistureWarningSettings?.ToSettings(),
                TemperatureWarningSettings = temperatureWarningSettings?.ToSettings(),
                WaterFillLevelWarningSettings = waterFillLevelWarningSettings?.ToSettings(),
                HardwareSettings = hardwareSettings?.ToSettings(),
            };
        }

        public static ApplicationSettingsDto FromSettings(ApplicationSettings settings)
        {
            return new ApplicationSettingsDto()
            {
                automaticWateringSettings = AutomaticWateringSettingsDto.FromSettings(
                    settings.AutomaticWateringSettings),
                moistureWarningSettings = MoistureWarningSettingsDto.FromSettings(
                    settings.MoistureWarningSettings),
                temperatureWarningSettings = TemperatureWarningSettingsDto.FromSettings(
                    settings.TemperatureWarningSettings),
                waterFillLevelWarningSettings = WaterFillLevelWarningSettingsDto.FromSettings(
                    settings.WaterFillLevelWarningSettings),
                hardwareSettings = HardwareSettingsDto.FromSettings(
                    settings.HardwareSettings)
            };
        }
    }
}
