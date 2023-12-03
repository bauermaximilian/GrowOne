/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Core.Settings;
using System;

namespace GrowOne.Services.ConfigurationManager
{
    internal class ConfigurationManagerService : IService
    {
        private const string SettingsFileName = "GrowOne.cfg";

        private readonly object settingsLock = new();
        private ApplicationSettings applicationSettings;

        public ConfigurationManagerService(bool initializeSettings)
        {
            if (!initializeSettings)
            {
                try
                {
                    var newApplicationSettings = LoadConfiguration();

                    if (newApplicationSettings == null ||
                        newApplicationSettings.MoistureWarningSettings == null ||
                        newApplicationSettings.TemperatureWarningSettings == null ||
                        newApplicationSettings.HardwareSettings == null ||
                        newApplicationSettings.AutomaticWateringSettings == null)
                    {
                        throw new ArgumentNullException();
                    }
                    else
                    {
                        applicationSettings = newApplicationSettings;
                    }
                }
                catch (Exception exc)
                {
                    Log.Warning("Couldn't load settings from device.", exc);
                }
            }

            if (applicationSettings == null)
            {
                applicationSettings = null!;
                ResetSettings();
            }
        }

        public ApplicationSettings GetCurrentSettings()
        {
            lock (settingsLock)
            {
                return applicationSettings.Clone();
            }
        }

        public void UpdateSettings(ApplicationSettings applicationSettings)
        {
            lock (settingsLock)
            {
                this.applicationSettings = applicationSettings;
                SaveConfiguration(applicationSettings);
            }
        }

        public void ResetSettings()
        {
            lock (settingsLock)
            {
                Log.Debug("Initializing default settings...");
                applicationSettings = ApplicationSettings.CreateDefault();
                try
                {
                    SaveConfiguration(applicationSettings);
                }
                catch (Exception exc)
                {
                    Log.Error("Default settings couldn't be saved to device.", exc);
                }
            }
        }

        private static void SaveConfiguration(ApplicationSettings userSettings)
        {
            InternalStorage.Save(userSettings, SettingsFileName, true);
        }

        private static ApplicationSettings LoadConfiguration()
        {
            return (ApplicationSettings)InternalStorage.Load(SettingsFileName,
                typeof(ApplicationSettings));
        }

        public void Dispose()
        {
        }
    }
}
