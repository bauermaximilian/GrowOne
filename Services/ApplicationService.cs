/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using GrowOne.Hardware.Board.Esp32;
using GrowOne.Services.ConfigurationManager;
using GrowOne.Services.DeviceManager;
using GrowOne.Services.MeasurementManager;
using GrowOne.Services.SensorManager;
using GrowOne.Services.WebServer;
using System;
using System.Threading;

namespace GrowOne.Services
{
    public class ApplicationService : BackgroundService
    {
        private const int StopServiceTimeoutMs = 5000;

        public bool ResetSettingsOnInitialisation { get; set; }

        protected override void Run(CancellationToken token)
        {
            Log.Debug("Loading application configuration...");
            var configurationManager = new ConfigurationManagerService(
                ResetSettingsOnInitialisation);
            var settings = configurationManager.GetCurrentSettings();

            Log.Debug("Initializing sub-services...");
            var board = new Esp32Board();
            var deviceManager = new DeviceManagerService(settings.HardwareSettings!, this, board);
            var sensorManager = new SensorManagerService(deviceManager.GetSensorProviders());
            var measurementManager = new MeasurementManagerService(sensorManager,
                new MoistureControlledIrrigator(settings.AutomaticWateringSettings!, deviceManager),
                new TemperatureRangeWarner(settings.TemperatureWarningSettings!, deviceManager),
                new MoistureRangeWarner(settings.MoistureWarningSettings!, deviceManager));

            var webServer = new WebServerService(deviceManager, new FileRequestHandler(),
                new ApiRequestHandler(measurementManager, configurationManager, deviceManager));
            AutoResetEvent webServerStartedEvent = new(false);
            void webServer_ServerStarted(object sender, EventArgs args)
            {
                webServer.ServerStarted -= webServer_ServerStarted;
                webServerStartedEvent.Set();
            }
            webServer.ServerStarted += webServer_ServerStarted;

            Log.Debug("Starting sub-services...");
            deviceManager.PlayNotificationSound(Status.Ping);
            var serviceStoppedEvent = StartServices(
                deviceManager, sensorManager, measurementManager, webServer);            

            webServerStartedEvent.WaitOne();

            if (!serviceStoppedEvent.WaitOne(500, false))
            {
                Log.Debug("All services started!");
                deviceManager.PlayNotificationSound(Status.Enabled);
            }

            int eventIndex = WaitHandle.WaitAny(
                new WaitHandle[] { serviceStoppedEvent, token.WaitHandle });

            if (eventIndex == 0)
            {
                Log.Error("A sub-service stopped unexpectedly.");
                deviceManager.PlayNotificationSound(Status.Failure);
            }
            else
            {
                Log.Debug("Stopping services...");
                deviceManager.PlayNotificationSound(Status.Disabled);
            }
            Thread.Sleep(1000);
            
            StopServices(sensorManager, measurementManager, webServer, deviceManager);
            Log.Debug("Application and its sub-services stopped.");
        }

        private static ManualResetEvent StartServices(params IHostedService[] services)
        {
            ManualResetEvent serviceStoppedEvent = new(false);
            foreach (IHostedService service in services)
            {
                void stateChangeHandler(object sender, EventArgs args)
                {
                    if (sender is IHostedService senderService &&
                        !senderService.IsRunning)
                    {
                        senderService.StateChanged -= stateChangeHandler;
                        serviceStoppedEvent.Set();
                    }
                }
                service.StateChanged += stateChangeHandler;

                service.Start();
                Thread.Sleep(500);
            }
            return serviceStoppedEvent;
        }

        private static bool StopServices(params IHostedService[] services)
        {
            bool allServicesStopped = true;

            foreach (IHostedService service in services)
            {
                AutoResetEvent stateChangedEvent = new(false);
                void stateChangeHandler(object sender, EventArgs args)
                {
                    stateChangedEvent.Set();
                }
                service.StateChanged += stateChangeHandler;

                service.Stop();

                allServicesStopped &= stateChangedEvent.WaitOne(StopServiceTimeoutMs, false);

                service.StateChanged -= stateChangeHandler;
            }

            return allServicesStopped;
        }
    }
}
