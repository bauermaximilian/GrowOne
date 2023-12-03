/*
 * Copyright (c) 2022 Maximilian Bauer
 * Distributed under the GNU GPL v3. For full terms see the file "COPYING".
 */
#nullable enable

using GrowOne.Core;
using System;

namespace GrowOne.Services.MeasurementManager
{
    internal abstract class MeasurementRangeSupervisor : IMeasurementSupervisor
    {
        private readonly double minimumValue;
        private readonly double maximumValue;
        private readonly int cooldownSeconds;
        private readonly int reactionTresholdSeconds;

        private TimeSpan valueExceedsRangeSince = TimeSpan.Zero;
        private TimeSpan remainingActionCooldown = TimeSpan.Zero;

        public bool IsActive
        {
            get => isActive;
            set 
            {
                if (isActive != value)
                {
                    valueExceedsRangeSince = TimeSpan.Zero;
                    remainingActionCooldown = TimeSpan.Zero;
                    isActive = value;
                }
            }
        }
        private bool isActive;

        public abstract MeasurementType SupervisedType { get; }

        public MeasurementRangeSupervisor(double minimumValue, double maximumValue, 
            int cooldownSeconds, int reactionTresholdSeconds = 10)
        {
            this.minimumValue = minimumValue;
            this.maximumValue = maximumValue;
            this.cooldownSeconds = cooldownSeconds;
            this.reactionTresholdSeconds = reactionTresholdSeconds;
        }

        public virtual void Update(TimeSpan delta, Measurement? measurement)
        {
            if (measurement != null && measurement.Type != SupervisedType)
                throw new ArgumentException();

            if (!IsActive) return;

            if (measurement == null)
            {
                valueExceedsRangeSince = TimeSpan.Zero;
                remainingActionCooldown = TimeSpan.Zero;
                return;
            }

            var measurementValue = (measurement.Type.Unit == Unit.Percent) ?
                (measurement.Value / 100) : measurement.Value;
            if (measurementValue < minimumValue || measurementValue > maximumValue)
            {
                valueExceedsRangeSince += delta;
            }
            else
            {
                valueExceedsRangeSince = TimeSpan.Zero;
            }

            if (remainingActionCooldown > TimeSpan.Zero)
            {
                long remainingActionCooldownTicks = 
                    remainingActionCooldown.Ticks - delta.Ticks;
                if (remainingActionCooldownTicks >= 0)
                {
                    remainingActionCooldown = 
                        TimeSpan.FromTicks(remainingActionCooldownTicks);
                }
                else
                {
                    remainingActionCooldown = TimeSpan.Zero;
                }
            }

            if (valueExceedsRangeSince.TotalSeconds > reactionTresholdSeconds &&
                remainingActionCooldown.TotalSeconds == 0)
            {
                remainingActionCooldown = TimeSpan.FromSeconds(cooldownSeconds);
                PerformRangeExceededAction(measurement);
            }
        }

        protected abstract void PerformRangeExceededAction(Measurement measurement);
    }
}
