/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common;
using GrowOne.Hardware.Sensors;
using System;
using System.Collections;
using System.Threading;

namespace GrowOne.Services.SensorManager
{
    internal class SensorManagerService : BackgroundService
    {
        /// <summary>
        /// Defines the amount of milliseconds between each sensor update.
        /// </summary>
        private const int UpdateSensorInvervalMs = 250;

        /// <summary>
        /// Defines the amount of sensor updates after which the actual
        /// <see cref="Measurements"/> will also be updated.
        /// </summary>
        private const int UpdateMeasurementsAfterSensorUpdateAmount = 4;

        private readonly ISensorProvider[] sensorProviders;

        /// <summary>
        /// Gets the current <see cref="Measurement"/>s for every <see cref="ISensor"/>
        /// managed by this instance.
        /// </summary>
        /// <remarks>
        /// These values are updated in specific intervals in the background and therefore
        /// don't always represent the most recent value.
        /// </remarks>
        public Measurement[] Measurements { get; private set; } = new Measurement[0];

        /// <summary>
        /// Occurs after the <see cref="Measurements"/> have been updated.
        /// </summary>
        public event EventHandler? MeasurementsUpdated;

        /// <summary>
        /// Initializes a new instance of the <see cref="SensorManagerService"/> class.
        /// </summary>
        /// <param name="sensorProviders">
        /// The <see cref="ISensorProvider"/> instances providing the <see cref="ISensor"/>
        /// instances which will be managed by this instance.
        /// </param>
        public SensorManagerService(params ISensorProvider[] sensorProviders)
        {
            this.sensorProviders = sensorProviders;
        }

        protected override void Run(CancellationToken token)
        {
            ISensor[] sensors = InitializeSensors(sensorProviders);
            DateTime lastSensorUpdate = DateTime.UtcNow;
            int sensorUpdatesWithoutMeasurementUpdates = 0;
            
            while (!token.IsCancellationRequested)
            {
                TimeSpan lastSensorUpdateDelta = DateTime.UtcNow - lastSensorUpdate;
                bool updateMeasurements = sensorUpdatesWithoutMeasurementUpdates >
                    UpdateMeasurementsAfterSensorUpdateAmount;

                ArrayList newMeasurementsList = new();   
                foreach (ISensor sensor in sensors)
                {
                    try
                    {
                        sensor.Update(lastSensorUpdateDelta);
                        Measurement? currentMeasurement = sensor.CurrentMeasurement;
                        if (currentMeasurement != null)
                        {
                            newMeasurementsList.Add(currentMeasurement);
                        }
                    }
                    catch (Exception exc)
                    {
                        Log.Warning($"Sensor \"{sensor.GetType()}\" couldn't be updated.", exc);
                    }
                }

                lastSensorUpdate = DateTime.UtcNow;
                if (newMeasurementsList.Count > 0)
                {
                    sensorUpdatesWithoutMeasurementUpdates = 0;
                    Measurement[] newMeasurements = new Measurement[newMeasurementsList.Count];
                    int i = 0;
                    foreach (Measurement measurement in newMeasurementsList)
                    {
                        newMeasurements[i++] = measurement;
                    }
                    Measurements = newMeasurements;
                    MeasurementsUpdated?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    sensorUpdatesWithoutMeasurementUpdates++;
                }

                token.WaitHandle.WaitOne(UpdateSensorInvervalMs, false);
            }

            foreach (ISensor sensor in sensors)
            {
                sensor.Dispose();
            }
        }

        private static ISensor[] InitializeSensors(ISensorProvider[] sensorProviders)
        {
            int sensorCount = 0;
            foreach (ISensorProvider sensorProvider in sensorProviders)
            {
                sensorCount += sensorProvider.SensorCount;
            }

            ArrayList sensors = new();
            foreach (ISensorProvider sensorProvider in sensorProviders)
            {
                for (int i = 0; i < sensorProvider.SensorCount; i++)
                {
                    try
                    {
                        sensors.Add(sensorProvider.OpenSensor(i));
                    }
                    catch (Exception exc)
                    {
                        Log.Error("Couldn't open sensor " +
                            $"#{i} of \"{sensorProvider.GetType().Name}\".", exc);
                    }
                }
            }

            ISensor[] sensorsArray = new ISensor[sensors.Count];
            int s = 0;
            foreach(ISensor sensor in sensors) sensorsArray[s++] = sensor;
            return sensorsArray;
        }
    }
}
