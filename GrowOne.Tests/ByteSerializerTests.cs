using GrowOne.Core;
using GrowOne.Core.Settings;
using nanoFramework.TestFramework;

namespace GrowOne.Tests
{
    // Arrange, Act, Assert
    [TestClass]
    public class ByteSerializerTests
    {
        private static MoistureWarningSettings TestData_MoistureWarningSettings => new()
        {
            Enabled = true,
            MaximumMoisture = 1,
            MinimumMoisture = 0.5f
        };

        private static HardwareSettings TestData_HardwareSettings => new()
        {
            BuzzerPin = 1,
            Dht22EchoPin = 2,
            Dht22TriggerPin = 3,
            Hcsr04EchoPin = 4,
            Hcsr04MaximumValueCm = 5,
            Hcsr04MinimumValueCm = 6,
            Hcsr04TriggerPin = 7,
            IrrigatorSwitchPin = 8,
            MoistureSensorPin = 9,
            SensorSwitchPin = 10,
            Password = "MyFirstPassword"
        };

        private static ApplicationSettings TestData_ApplicationSettings => new()
        {
            HardwareSettings = TestData_HardwareSettings,
            MoistureWarningSettings = TestData_MoistureWarningSettings
        };

        private static byte[] TestData_MoistureWarningSettingsBuffer_WithChecksum => new byte[] {
            1,
            0, 0, 0, 63,
            0, 0, 128, 63,
            95, 230, 154, 204
        };
        
        private static byte[] TestData_MoistureWarningSettingsBuffer_WithoutChecksum => new byte[] {
            1,
            0, 0, 0, 63,
            0, 0, 128, 63
        };

        [TestMethod]
        public void SerializeWithChecksum_MoistureWarningSettings_ReturnsCorrectByteArray()
        {            
            var settings = TestData_MoistureWarningSettings;
            var expectedBuffer = TestData_MoistureWarningSettingsBuffer_WithChecksum;

            var actualBuffer = ByteSerializer.Serialize(settings);

            Assert.IsTrue(expectedBuffer.SequenceEqual(actualBuffer));
        }

        [TestMethod]
        public void SerializeWithoutChecksum_MoistureWarningSettings_ReturnsCorrectByteArray()
        {
            var settings = TestData_MoistureWarningSettings;
            var expectedBuffer = TestData_MoistureWarningSettingsBuffer_WithoutChecksum;

            var actualBuffer = ByteSerializer.Serialize(settings, false);

            Assert.IsTrue(expectedBuffer.SequenceEqual(actualBuffer));
        }

        [TestMethod]
        public void DeserializeWithChecksum_MoistureWarningSettings_ReturnsCorrectByteArray()
        {
            var buffer = TestData_MoistureWarningSettingsBuffer_WithChecksum;
            var expectedSettings = TestData_MoistureWarningSettings;

            int i = 0;
            var actualSettings = (MoistureWarningSettings)ByteSerializer.Deserialize(buffer, ref i, 
                typeof(MoistureWarningSettings), true);

            AssertMoistureWarningSettingsAreEqual(expectedSettings, actualSettings);
        }

        [TestMethod]
        public void DeserializeWithoutChecksum_MoistureWarningSettings_ReturnsCorrectByteArray()
        {
            var buffer = TestData_MoistureWarningSettingsBuffer_WithoutChecksum;
            var expectedSettings = TestData_MoistureWarningSettings;

            int i = 0;
            var actualSettings = (MoistureWarningSettings)ByteSerializer.Deserialize(buffer, ref i,
                typeof(MoistureWarningSettings), false);

            AssertMoistureWarningSettingsAreEqual(expectedSettings, actualSettings);
        }

        [TestMethod]
        public void SerializeAndDeserializeWithChecksum_HardwareSettings_ReturnsSameSettings()
        {
            var expectedSettings = TestData_HardwareSettings;

            var buffer = ByteSerializer.Serialize(expectedSettings, true);
            int i = 0;
            var actualSettings = (HardwareSettings)ByteSerializer.Deserialize(buffer,
                ref i, typeof(HardwareSettings), true);

            AssertHardwareSettingsAreEqual(expectedSettings, actualSettings);
        }

        [TestMethod]
        public void SerializeAndDeserializeWithoutChecksum_HardwareSettings_ReturnsSameSettings()
        {
            var expectedSettings = TestData_HardwareSettings;

            var buffer = ByteSerializer.Serialize(expectedSettings, false);
            int i = 0;
            var actualSettings = (HardwareSettings)ByteSerializer.Deserialize(buffer,
                ref i, typeof(HardwareSettings), false);

            AssertHardwareSettingsAreEqual(expectedSettings, actualSettings);
        }

        [TestMethod]
        public void SerializeAndDeserializeWithChecksum_ApplicationSettings_ReturnsSameSettings()
        {
            var expectedSettings = TestData_ApplicationSettings;

            var buffer = ByteSerializer.Serialize(expectedSettings, true);
            int i = 0;
            var actualSettings = (ApplicationSettings)ByteSerializer.Deserialize(buffer,
                ref i, typeof(ApplicationSettings), true);

            AssertHardwareSettingsAreEqual(expectedSettings.HardwareSettings, 
                actualSettings.HardwareSettings);
            AssertMoistureWarningSettingsAreEqual(expectedSettings.MoistureWarningSettings,
                actualSettings.MoistureWarningSettings);
            Assert.IsNull(actualSettings.WaterFillLevelWarningSettings);
            Assert.IsNull(actualSettings.TemperatureWarningSettings);
            Assert.IsNull(actualSettings.AutomaticWateringSettings);
        }

        [TestMethod]
        public void SerializeAndDeserializeWithoutChecksum_ApplicationSettings_ReturnsSameSettings()
        {
            var expectedSettings = TestData_ApplicationSettings;

            var buffer = ByteSerializer.Serialize(expectedSettings, false);
            int i = 0;
            var actualSettings = (ApplicationSettings)ByteSerializer.Deserialize(buffer,
                ref i, typeof(ApplicationSettings), false);

            AssertHardwareSettingsAreEqual(expectedSettings.HardwareSettings,
                actualSettings.HardwareSettings);
            AssertMoistureWarningSettingsAreEqual(expectedSettings.MoistureWarningSettings,
                actualSettings.MoistureWarningSettings);
            Assert.IsNull(actualSettings.WaterFillLevelWarningSettings);
            Assert.IsNull(actualSettings.TemperatureWarningSettings);
            Assert.IsNull(actualSettings.AutomaticWateringSettings);
        }

        private static void AssertMoistureWarningSettingsAreEqual(MoistureWarningSettings expectedSettings,
            MoistureWarningSettings actualSettings)
        {
            Assert.AreEqual(expectedSettings.Enabled, actualSettings.Enabled);
            Assert.AreEqual(expectedSettings.MaximumMoisture, actualSettings.MaximumMoisture);
            Assert.AreEqual(expectedSettings.MinimumMoisture, actualSettings.MinimumMoisture);
        }

        private static void AssertHardwareSettingsAreEqual(HardwareSettings expectedSettings, 
            HardwareSettings actualSettings)
        {
            Assert.AreEqual(expectedSettings.BuzzerPin, actualSettings.BuzzerPin);
            Assert.AreEqual(expectedSettings.Dht22EchoPin, actualSettings.Dht22EchoPin);
            Assert.AreEqual(expectedSettings.Dht22TriggerPin, actualSettings.Dht22TriggerPin);
            Assert.AreEqual(expectedSettings.Hcsr04EchoPin, actualSettings.Hcsr04EchoPin);
            Assert.AreEqual(expectedSettings.Hcsr04MaximumValueCm, actualSettings.Hcsr04MaximumValueCm);
            Assert.AreEqual(expectedSettings.Hcsr04MinimumValueCm, actualSettings.Hcsr04MinimumValueCm);
            Assert.AreEqual(expectedSettings.Hcsr04TriggerPin, actualSettings.Hcsr04TriggerPin);
            Assert.AreEqual(expectedSettings.IrrigatorSwitchPin, actualSettings.IrrigatorSwitchPin);
            Assert.AreEqual(expectedSettings.MoistureSensorPin, actualSettings.MoistureSensorPin);
            Assert.AreEqual(expectedSettings.SensorSwitchPin, actualSettings.SensorSwitchPin);
            Assert.AreEqual(expectedSettings.Password, actualSettings.Password);
        }
    }
}
