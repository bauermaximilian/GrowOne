/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

namespace GrowOne.Common
{
    public static class MeasurementExtensions
    {
        public static bool TryGetMeasurement(this Measurement[] measurements,
            MeasurementType measurementType, out Measurement measurement)
        {
            foreach (Measurement measurementCandidate in measurements)
            {
                if (measurementCandidate.Type == measurementType)
                {
                    measurement = measurementCandidate;
                    return true;
                }
            }

            measurement = null!;
            return false;
        }
    }
}
