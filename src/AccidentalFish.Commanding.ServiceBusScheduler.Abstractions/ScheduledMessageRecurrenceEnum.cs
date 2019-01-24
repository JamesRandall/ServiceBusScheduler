namespace AccidentalFish.Commanding.ServiceBusScheduler.Abstractions
{
    /// <summary>
    /// Recurrence type
    /// </summary>
    public enum ScheduledMessageRecurrenceEnum
    {
        // Does not recur
        DoesNotRecur = 0,
        // Daily recurrence
        Daily = 1,
        // Hourly recurrence
        Hourly = 2,
        // Minute recurrence
        Minute = 3
    }
}