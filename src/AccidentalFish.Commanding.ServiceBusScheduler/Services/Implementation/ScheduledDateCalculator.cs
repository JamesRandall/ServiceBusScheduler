using System;
using System.Collections.Generic;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Services.Implementation
{
    class ScheduledDateCalculator : IScheduledDateCalculator
    {
        private static readonly Dictionary<ScheduledMessageRecurrenceEnum, Func<DateTime, DateTime>> Calculators = new Dictionary<ScheduledMessageRecurrenceEnum, Func<DateTime, DateTime>>()
        {
            { ScheduledMessageRecurrenceEnum.Daily, dt => dt.AddDays(1) },
            { ScheduledMessageRecurrenceEnum.Hourly, dt => dt.AddHours(1) },
            { ScheduledMessageRecurrenceEnum.Minute, dt => dt.AddMinutes(1) }
        };

        public DateTime NextScheduledAtUtc(DateTime lastScheduledAtUtc, ScheduledMessageRecurrenceEnum recurrenceType)
        {
            if (recurrenceType == ScheduledMessageRecurrenceEnum.DoesNotRecur)
            {
                throw new NotSupportedException("Only daily recurrence is currently supported");
            }

            if (Calculators.TryGetValue(recurrenceType, out var adjustmentFunc))
            {
                return adjustmentFunc(lastScheduledAtUtc);
            }

            throw new NotSupportedException($"Recurrence type {recurrenceType} has no registered calculator");
        }
    }
}
