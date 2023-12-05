/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core.Settings;

namespace GrowOne.Services.WebServer.Contracts
{
#pragma warning disable IDE1006
    public class HardwareSettingsDto
    {
        public int msp { get; set; }

        public int dep { get; set; }

        public int dtp { get; set; }

        public int isp { get; set; }

        public int ssp { get; set; }

        public int bp { get; set; }

        public int hep { get; set; }

        public int htp { get; set; }

        public int hmin { get; set; }

        public int hmax { get; set; }

        public string? pass { get; set; }

        public HardwareSettings ToSettings()
        {
            return new HardwareSettings()
            {
                MoistureSensorPin = msp,
                Dht22EchoPin = dep,
                Dht22TriggerPin = dtp,
                IrrigatorSwitchPin = isp,
                SensorSwitchPin = ssp,
                BuzzerPin = bp,
                Hcsr04EchoPin = hep,
                Hcsr04TriggerPin = htp,
                Hcsr04MinimumValueCm = hmin,
                Hcsr04MaximumValueCm = hmax,
                Password = pass
            };
        }

        public static HardwareSettingsDto? FromSettings(HardwareSettings? settings)
        {
            if (settings == null) return null;
            else return new HardwareSettingsDto()
            {
                msp = settings.MoistureSensorPin,
                dep = settings.Dht22EchoPin,
                dtp = settings.Dht22TriggerPin,
                isp = settings.IrrigatorSwitchPin,
                ssp = settings.SensorSwitchPin,
                bp = settings.BuzzerPin,
                hep = settings.Hcsr04EchoPin,
                htp = settings.Hcsr04TriggerPin,
                hmin = settings.Hcsr04MinimumValueCm,
                hmax = settings.Hcsr04MaximumValueCm,
                pass = settings.Password
            };
        }
    }
}
