/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Common.Settings
{
    public class HardwareSettings
    {
        public int MoistureSensorPin { get; set; } = 34;

        public int Dht22EchoPin { get; set; } = 12;

        public int Dht22TriggerPin { get; set; } = 14;

        public int Hcsr04EchoPin { get; set; } = 25;

        public int Hcsr04TriggerPin { get; set; } = 26;

        public int Hcsr04MinimumValueCm { get; set; } = 2;

        public int Hcsr04MaximumValueCm { get; set; } = 17;

        public int IrrigatorSwitchPin { get; set; } = 23;

        public int SensorSwitchPin { get; set; } = 22;

        public int BuzzerPin { get; set; } = 18;

        public string? Password { get; set; } = null;

        public HardwareSettings Clone()
        {
            return new HardwareSettings()
            {
                MoistureSensorPin = MoistureSensorPin,
                Dht22EchoPin = Dht22EchoPin,
                Dht22TriggerPin = Dht22TriggerPin,
                Hcsr04EchoPin = Hcsr04EchoPin,
                Hcsr04TriggerPin = Hcsr04TriggerPin,
                Hcsr04MinimumValueCm = Hcsr04MinimumValueCm,
                Hcsr04MaximumValueCm = Hcsr04MaximumValueCm,
                IrrigatorSwitchPin = IrrigatorSwitchPin,
                SensorSwitchPin = SensorSwitchPin,
                BuzzerPin = BuzzerPin,
                Password = Password
            };
        }
    }
}
