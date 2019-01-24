using System;
using System.Threading.Tasks;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Abstractions
{
    /// <summary>
    /// The primary interface for the scheduler - allows schedules to be setup and 
    /// </summary>
    public interface ICommandScheduler
    {
        /// <summary>
        /// Schedules a command to run once at a specific date and time
        /// </summary>
        /// <param name="frequency">The recurrence type for the message</param>
        /// <param name="command">The command</param>
        /// <param name="scheduledAtUtc">The schedule date and time</param>
        /// <param name="optionalMessageId">If specified then this will be used for the service bus message id, if null or whitespace then an ID will be generated</param>
        /// <returns>The message ID</returns>
        Task<string> ScheduleCommand(ScheduledMessageRecurrenceEnum frequency, ICommand<bool> command, DateTime? scheduledAtUtc, string optionalMessageId = null);

        /// <summary>
        /// Cancels a scheduled command
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        Task Cancel(string commandId);
    }
}
