/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Common
{
    public class MeasurementType
    {
        public static MeasurementType Temperature { get; } =
            new MeasurementType("Temperature", Unit.Celsius);

        public static MeasurementType Humidity { get; } =
            new MeasurementType("Humidity", Unit.Percent);

        public static MeasurementType Moisture { get; } =
            new MeasurementType("Moisture", Unit.Percent);

        public static MeasurementType Distance { get; } =
            new MeasurementType("Distance", Unit.Meter);

        public static MeasurementType FillLevel { get; } =
            new MeasurementType("Water fill level", Unit.Percent);

        public string Label { get; }

        public Unit Unit { get; }

        public MeasurementType(string label, Unit unit)
        {
            Label = label;
            Unit = unit;
        }

        public override bool Equals(object obj)
        {
            return obj is MeasurementType type &&
                   Label == type.Label &&
                   Unit == type.Unit;
        }

        public override int GetHashCode()
        {
            int hashCode = -1553767860;
            hashCode = hashCode * -1521134295 + Label.GetHashCode();
            hashCode = hashCode * -1521134295 + Unit.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(MeasurementType? left, MeasurementType? right)
        {
            return (left is null && right is null) ||
                (left is not null && right is not null && left.Equals(right));
        }

        public static bool operator !=(MeasurementType? left, MeasurementType? right)
        {
            return !(left == right);
        }
    }
}
