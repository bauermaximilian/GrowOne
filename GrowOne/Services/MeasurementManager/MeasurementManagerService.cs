/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using GrowOne.Services.SensorManager;
using System;
using System.Threading;

namespace GrowOne.Services.MeasurementManager
{
    internal class MeasurementManagerService : BackgroundService
    {
        public MeasurementStatistics? Temperature { get; private set; } 

        public MeasurementStatistics? Humidity { get; private set; }

        public MeasurementStatistics? Moisture { get; private set; }

        public MeasurementStatistics? FillLevel { get; private set; }

        private readonly SensorManagerService sensorManager;
        private readonly IMeasurementSupervisor[] measurementSupervisors;
        private readonly object measurementLock = new();

        public MeasurementManagerService(SensorManagerService sensorManager,
            params IMeasurementSupervisor[] measurementSupervisors)
        {
            this.sensorManager = sensorManager;
            this.measurementSupervisors = measurementSupervisors;
            sensorManager.MeasurementsUpdated += SensorManager_MeasurementsUpdated;
        }

        public void Reset()
        {
            Temperature = null;
            Humidity = null;
            Moisture = null;
            FillLevel = null;
        }

        protected override void Run(CancellationToken token)
        {
            DateTime lastUpdate = DateTime.UtcNow;

            while (!token.IsCancellationRequested)
            {
                TimeSpan delta = DateTime.UtcNow - lastUpdate;
                lastUpdate = DateTime.UtcNow;
                foreach (IMeasurementSupervisor measurementSupervisor in measurementSupervisors)
                {
                    try
                    {
                        if (measurementSupervisor.SupervisedType == MeasurementType.Temperature)
                        {
                            measurementSupervisor.Update(delta, Temperature?.CurrentValue);
                        }
                        else if (measurementSupervisor.SupervisedType == MeasurementType.Humidity)
                        {
                            measurementSupervisor.Update(delta, Humidity?.CurrentValue);
                        }
                        else if (measurementSupervisor.SupervisedType == MeasurementType.Moisture)
                        {
                            measurementSupervisor.Update(delta, Moisture?.CurrentValue);
                        }
                        else if (measurementSupervisor.SupervisedType == MeasurementType.FillLevel)
                        {
                            measurementSupervisor.Update(delta, FillLevel?.CurrentValue);
                        }
                    }
                    catch (Exception exc)
                    {
                        Log.Warning("Couldn't update " +
                            $"\"{measurementSupervisor.SupervisedType.Label}\" supervisor.", exc);
                    }
                }
                Thread.Sleep(500);
            }
        }

        private void SensorManager_MeasurementsUpdated(object sender, EventArgs e)
        {
            lock (measurementLock)
            {
                if (sensorManager.Measurements.TryGetMeasurement(MeasurementType.Temperature,
                    out Measurement temperatureMeasurement))
                {
                    Temperature = Temperature?.WithMeasurement(temperatureMeasurement) ??
                        new MeasurementStatistics(temperatureMeasurement);
                }

                if (sensorManager.Measurements.TryGetMeasurement(MeasurementType.Humidity,
                    out Measurement humidityMeasurement))
                {
                    Humidity = Humidity?.WithMeasurement(humidityMeasurement) ??
                        new MeasurementStatistics(humidityMeasurement);
                }

                if (sensorManager.Measurements.TryGetMeasurement(MeasurementType.Moisture,
                    out Measurement moistureMeasurement))
                {
                    Moisture = Moisture?.WithMeasurement(moistureMeasurement) ??
                        new MeasurementStatistics(moistureMeasurement);
                }

                if (sensorManager.Measurements.TryGetMeasurement(MeasurementType.FillLevel,
                    out Measurement fillLevelMeasurement))
                {
                    FillLevel = FillLevel?.WithMeasurement(fillLevelMeasurement) ??
                        new MeasurementStatistics(fillLevelMeasurement);
                }
            }
        }
    }
}
