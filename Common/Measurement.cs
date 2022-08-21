/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using System;
using System.Collections;

namespace GrowOne.Common
{
    /// <summary>
    /// Represents a measurement from a <see cref="Hardware.Sensors.ISensor"/>.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Gets the <see cref="MeasurementType"/> of the current instance.
        /// </summary>
        public MeasurementType Type { get; }

        /// <summary>
        /// Gets the value of the current instance.
        /// </summary>
        public double Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Measurement"/> class.
        /// </summary>
        /// <param name="value">
        /// The value of the measurement.
        /// </param>
        /// <param name="type">
        /// The type of the measurement.
        /// </param>
        public Measurement(double value, MeasurementType type)
        {
            Value = value;
            Type = type;
        }

        public override string ToString()
        {
            return ToString(1);
        }

        public string ToString(int maxDecimalPlaces)
        {
            return $"{DoubleToDecimalClampedString(Value, maxDecimalPlaces)}{Type.Unit.ToUnitSymbol()}";
        }

        private static string DoubleToDecimalClampedString(double value, int maxDecimalPlaces)
        {
            string valueStringRaw = value.ToString();
            int decimalSeparatorIndex = valueStringRaw.IndexOf('.');
            if (decimalSeparatorIndex < 0) return valueStringRaw;
            else return valueStringRaw.Substring(0, 
                decimalSeparatorIndex + maxDecimalPlaces + (maxDecimalPlaces > 0 ? 1 : 0));
        }
    }
}
