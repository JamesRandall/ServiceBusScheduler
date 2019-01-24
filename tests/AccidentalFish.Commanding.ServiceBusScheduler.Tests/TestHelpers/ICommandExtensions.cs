using System;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.TestHelpers
{
    // ReSharper disable once InconsistentNaming
    public static class ICommandExtensions
    {
        public static ScheduledMessage CreateScheduledMessage(this ICommand command, string messageId = null)
        {
            return new ScheduledMessage
            {
                Id = messageId ?? Guid.NewGuid().ToString(),
                LastScheduledAtUtc = null,
                LastExecutedAtUtc = null,
                Recurrence = ScheduledMessageRecurrenceEnum.Daily,
                ScheduledAtUtc = DateTime.UtcNow.AddHours(1),
                WrappedCommandFullTypeName = command.GetType().FullName,
                WrappedCommandJson = JsonConvert.SerializeObject(command)
            };
        }
    }
}
