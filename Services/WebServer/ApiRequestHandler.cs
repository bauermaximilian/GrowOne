/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using GrowOne.Services.ConfigurationManager;
using GrowOne.Services.DeviceManager;
using GrowOne.Services.MeasurementManager;
using GrowOne.Services.WebServer.Contracts;
using nanoFramework.Json;
using System;
using System.Collections;
using System.Net;

namespace GrowOne.Services.WebServer
{
    internal class ApiRequestHandler : RequestHandler
    {
        private const string DefaultUserName = "GrowOne";
        private const string SettingsUpdateError = "The settings couldn't be updated. ";

        private readonly string? userPassword;
        private readonly MeasurementManagerService measurementManager;
        private readonly ConfigurationManagerService configurationManager;
        private readonly DeviceManagerService deviceManager;

        public ApiRequestHandler(MeasurementManagerService measurementManager,
            ConfigurationManagerService configurationManager,
            DeviceManagerService deviceManager)
        {
            userPassword = configurationManager.GetCurrentSettings().HardwareSettings?.Password ?? string.Empty;
            this.measurementManager = measurementManager;
            this.configurationManager = configurationManager;
            this.deviceManager = deviceManager;
        }

        public override void HandleRequest(HttpListenerContext context)
        {
            if ((context.Request.Credentials == null && !string.IsNullOrEmpty(userPassword)) ||
                context.Request.Credentials?.UserName != DefaultUserName ||
                context.Request.Credentials?.Password != userPassword)
            {
                context.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"API access\"");
                WebServerUtils.SendJsonResponse(context.Response, 401,
                    new StatusDescriptorDto(401, "Access denied."));
            }
            else
            {
                base.HandleRequest(context);
            }
        }

        [RequestRoute("api/", WebServerUtils.HttpMethodGet)]
        protected void HandleRequestRoot(HttpListenerContext context)
        {
            WebServerUtils.SendJsonResponse(context.Response, 200,
                new StatusDescriptorDto(200, "Hello!"));
        }

        [RequestRoute("api/measurements", WebServerUtils.HttpMethodGet)]
        protected void HandleRequestMeasurements(HttpListenerContext context)
        {
            ArrayList measurementsList = new();
            
            if (measurementManager.Temperature != null)
            {
                measurementsList.Add(measurementManager.Temperature);
            }
            if (measurementManager.Humidity != null)
            {
                measurementsList.Add(measurementManager.Humidity);
            }
            if (measurementManager.Moisture != null)
            {
                measurementsList.Add(measurementManager.Moisture);
            }
            if (measurementManager.FillLevel != null)
            {
                measurementsList.Add(measurementManager.FillLevel);
            }

            MeasurementDto[] measurementDtos = new MeasurementDto[measurementsList.Count];
            int i = 0;
            foreach (MeasurementStatistics measurement in measurementsList)
            {
                measurementDtos[i++] = new MeasurementDto()
                {
                    label = measurement.MeasurementType.Label,
                    currentValue = measurement.CurrentValue.ToString(),
                    statistics = $"{measurement.MinimumValue.ToString(0)} min.\n" +
                        $"{measurement.MaximumValue.ToString(0)} max."
                };
            }
            //measurementDtos[i] = new MeasurementDto()
            //{
            //    label = "Watering",
            //    currentValue = deviceManager.LastWateringFormatted,
            //    statistics = "since\nwatering",
            //};

            WebServerUtils.SendJsonResponse(context.Response, 200, measurementDtos);
        }

        [RequestRoute("api/action/water/", WebServerUtils.HttpMethodPost)]
        protected void HandleWaterManually(HttpListenerContext context)
        {
            string waterTimeSecondsString = 
                context.Request.RawUrl.Substring("api/action/water/".Length + 1);
            if (int.TryParse(waterTimeSecondsString, out int waterTimeSeconds) &&
                waterTimeSeconds > 0)
            {
                deviceManager.PerformWatering(TimeSpan.FromSeconds(waterTimeSeconds));
                WebServerUtils.SendEmptyResponse(context.Response);
            }
            else
            {
                WebServerUtils.SendJsonResponse(context.Response, 400,
                    new StatusDescriptorDto(400, "The specified time was no valid " +
                    "(positive) integer."));
            }
        }

        [RequestRoute("api/measurements", WebServerUtils.HttpMethodDelete)]
        protected void HandleResetMeasurementStatistics(HttpListenerContext context)
        {
            measurementManager.Reset();
            WebServerUtils.SendEmptyResponse(context.Response, 205);
        }

        [RequestRoute("api/action/soundtest", WebServerUtils.HttpMethodPost)]
        protected void HandleTestNotificationSound(HttpListenerContext context)
        {
            deviceManager.PlayNotificationSound(Status.Default);
            WebServerUtils.SendEmptyResponse(context.Response);
        }

        [RequestRoute("api/action/restart", WebServerUtils.HttpMethodPost)]
        protected void HandleRestartDevice(HttpListenerContext context)
        {
            deviceManager.RestartDevice();
            WebServerUtils.SendEmptyResponse(context.Response);
        }

        [RequestRoute("api/configuration", WebServerUtils.HttpMethodPut)]
        protected void HandleUpdateConfiguration(HttpListenerContext context)
        {
            try
            {
                var settingsDto = (ApplicationSettingsDto)JsonConvert.DeserializeObject(
                    context.Request.InputStream, typeof(ApplicationSettingsDto));
                var settings = settingsDto.ToSettings();
                configurationManager.UpdateSettings(settings);

                WebServerUtils.SendEmptyResponse(context.Response);
            }
            catch (Exception exc)
            {
                WebServerUtils.SendJsonResponse(context.Response, 400,
                    new StatusDescriptorDto(400, SettingsUpdateError + exc.Message));
            }
        }

        [RequestRoute("api/configuration/automaticwatering", WebServerUtils.HttpMethodPut)]
        protected void HandleUpdateConfigurationAutomaticWatering(HttpListenerContext context)
        {
            try
            {
                var settingsSegmentDto = 
                    (AutomaticWateringSettingsDto)JsonConvert.DeserializeObject(
                    context.Request.InputStream, typeof(AutomaticWateringSettingsDto));
                var settingsSegment = settingsSegmentDto.ToSettings();
                var currentSettings = configurationManager.GetCurrentSettings();

                currentSettings.AutomaticWateringSettings = settingsSegment;
                configurationManager.UpdateSettings(currentSettings);

                WebServerUtils.SendEmptyResponse(context.Response);
            }
            catch (Exception exc)
            {
                WebServerUtils.SendJsonResponse(context.Response, 400,
                    new StatusDescriptorDto(400, SettingsUpdateError + exc.Message));
            }
        }

        [RequestRoute("api/configuration/hardware", WebServerUtils.HttpMethodPut)]
        protected void HandleUpdateConfigurationHardware(HttpListenerContext context)
        {
            try
            {
                var settingsSegmentDto =
                    (HardwareSettingsDto)JsonConvert.DeserializeObject(
                    context.Request.InputStream, typeof(HardwareSettingsDto));
                var settingsSegment = settingsSegmentDto.ToSettings();
                var currentSettings = configurationManager.GetCurrentSettings();

                currentSettings.HardwareSettings = settingsSegment;
                configurationManager.UpdateSettings(currentSettings);

                WebServerUtils.SendEmptyResponse(context.Response);
            }
            catch (Exception exc)
            {
                WebServerUtils.SendJsonResponse(context.Response, 400,
                    new StatusDescriptorDto(400, SettingsUpdateError + exc.Message));
            }
        }

        [RequestRoute("api/configuration/moisturewarning", WebServerUtils.HttpMethodPut)]
        protected void HandleUpdateConfigurationMoistureWarning(HttpListenerContext context)
        {
            try
            {
                var settingsSegmentDto =
                    (MoistureWarningSettingsDto)JsonConvert.DeserializeObject(
                    context.Request.InputStream, typeof(MoistureWarningSettingsDto));
                var settingsSegment = settingsSegmentDto.ToSettings();
                var currentSettings = configurationManager.GetCurrentSettings();

                currentSettings.MoistureWarningSettings = settingsSegment;
                configurationManager.UpdateSettings(currentSettings);

                WebServerUtils.SendEmptyResponse(context.Response);
            }
            catch (Exception exc)
            {
                WebServerUtils.SendJsonResponse(context.Response, 400,
                    new StatusDescriptorDto(400, SettingsUpdateError + exc.Message));
            }
        }

        [RequestRoute("api/configuration/temperaturewarning", WebServerUtils.HttpMethodPut)]
        protected void HandleUpdateConfigurationTemperatureWarning(HttpListenerContext context)
        {
            try
            {
                var settingsSegmentDto =
                    (TemperatureWarningSettingsDto)JsonConvert.DeserializeObject(
                    context.Request.InputStream, typeof(TemperatureWarningSettingsDto));
                var settingsSegment = settingsSegmentDto.ToSettings();
                var currentSettings = configurationManager.GetCurrentSettings();

                currentSettings.TemperatureWarningSettings = settingsSegment;
                configurationManager.UpdateSettings(currentSettings);

                WebServerUtils.SendEmptyResponse(context.Response);
            }
            catch (Exception exc)
            {
                WebServerUtils.SendJsonResponse(context.Response, 400,
                    new StatusDescriptorDto(400, SettingsUpdateError + exc.Message));
            }
        }

        [RequestRoute("api/configuration/waterfilllevelwarning", WebServerUtils.HttpMethodPut)]
        protected void HandleUpdateConfigurationWaterFillLevelWarning(HttpListenerContext context)
        {
            try
            {
                var settingsSegmentDto =
                    (WaterFillLevelWarningSettingsDto)JsonConvert.DeserializeObject(
                    context.Request.InputStream, typeof(WaterFillLevelWarningSettingsDto));
                var settingsSegment = settingsSegmentDto.ToSettings();
                var currentSettings = configurationManager.GetCurrentSettings();

                currentSettings.WaterFillLevelWarningSettings = settingsSegment;
                configurationManager.UpdateSettings(currentSettings);

                WebServerUtils.SendEmptyResponse(context.Response);
            }
            catch (Exception exc)
            {
                WebServerUtils.SendJsonResponse(context.Response, 400,
                    new StatusDescriptorDto(400, SettingsUpdateError + exc.Message));
            }
        }

        [RequestRoute("api/configuration", WebServerUtils.HttpMethodGet)]
        protected void HandleGetConfiguration(HttpListenerContext context)
        {
            WebServerUtils.SendJsonResponse(context.Response, 200,
                ApplicationSettingsDto.FromSettings(configurationManager.GetCurrentSettings()));
        }
    }
}
