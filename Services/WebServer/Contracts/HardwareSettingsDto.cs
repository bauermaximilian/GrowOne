/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Common.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class HardwareSettingsDto
    {
        public int moistureSensorPin { get; set; }

        public int dht22EchoPin { get; set; }

        public int dht22TriggerPin { get; set; }

        public int irrigatorSwitchPin { get; set; }

        public int sensorSwitchPin { get; set; }

        public int buzzerPin { get; set; }

        public int hcsr04EchoPin { get; set; }

        public int hcsr04TriggerPin { get; set; }

        public int hcsr04MinimumValueCm { get; set; }

        public int hcsr04MaximumValueCm { get; set; }

        public string? password { get; set; }

        public HardwareSettings ToSettings()
        {
            return new HardwareSettings()
            {
                MoistureSensorPin = moistureSensorPin,
                Dht22EchoPin = dht22EchoPin,
                Dht22TriggerPin = dht22TriggerPin,
                IrrigatorSwitchPin = irrigatorSwitchPin,
                SensorSwitchPin = sensorSwitchPin,
                BuzzerPin = buzzerPin,
                Hcsr04EchoPin = hcsr04EchoPin,
                Hcsr04TriggerPin = hcsr04TriggerPin,
                Hcsr04MinimumValueCm = hcsr04MinimumValueCm,
                Hcsr04MaximumValueCm = hcsr04MaximumValueCm,
                Password = password
            };
        }

        public static HardwareSettingsDto? FromSettings(HardwareSettings? settings)
        {
            if (settings == null) return null;
            else return new HardwareSettingsDto()
            {
                moistureSensorPin = settings.MoistureSensorPin,
                dht22EchoPin = settings.Dht22EchoPin,
                dht22TriggerPin = settings.Dht22TriggerPin,
                irrigatorSwitchPin = settings.IrrigatorSwitchPin,
                sensorSwitchPin = settings.SensorSwitchPin,
                buzzerPin = settings.BuzzerPin,
                hcsr04EchoPin = settings.Hcsr04EchoPin,
                hcsr04TriggerPin = settings.Hcsr04TriggerPin,
                hcsr04MinimumValueCm = settings.Hcsr04MinimumValueCm,
                hcsr04MaximumValueCm = settings.Hcsr04MaximumValueCm,
                password = settings.Password
            };
        }
    }
}
