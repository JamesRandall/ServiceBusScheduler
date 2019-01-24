using System;
using System.Collections.Generic;
using System.Text;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Services
{
    interface IScheduledDateCalculator
    {
        DateTime NextScheduledAtUtc(DateTime lastScheduledAtUtc, ScheduledMessageRecurrenceEnum recurrenceType);
    }
}
