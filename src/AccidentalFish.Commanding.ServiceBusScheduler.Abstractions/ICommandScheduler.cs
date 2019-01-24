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
        /// Allows a command to be scheduled to be executed daily
        /// </summary>
        /// <param name="command">The command should return true if it wants to be rescheduled, false to stop.</param>
        /// <param name="initialExecutionDateTimeUtc">If specified then the command will be run every 24 hours with the first execution at this date and time, if null then the command will be executed immediately and then run every 24 hours after that</param>
        /// <param name="optionalMessageId">If specified then this will be used for the service bus message id, if null or whitespace then an ID will be generated</param>
        /// <returns>The message ID</returns>
        Task<string> ScheduleDailyCommand(ICommand<bool> command, DateTime initialExecutionDateTimeUtc, string optionalMessageId=null);

        /// <summary>
        /// Allows a command to be scheduled to be executed daily with the first execution to occur immediately
        /// </summary>
        /// <param name="command">The command should return true if it wants to be rescheduled, false to stop.</param>
        /// <param name="optionalMessageId">If specified then this will be used for the service bus message id, if null or whitespace then an ID will be generated</param>
        /// <returns>The message ID</returns>
        Task<string> ScheduleDailyCommand(ICommand<bool> command, string optionalMessageId=null);

        /// <summary>
        /// Allows a command to be scheduled to be executed once a minute
        /// </summary>
        /// <param name="command">The command should return true if it wants to be rescheduled, false to stop.</param>
        /// <param name="initialExecutionDateTimeUtc">If specified then the command will be run every 24 hours with the first execution at this date and time, if null then the command will be executed immediately and then run every 24 hours after that</param>
        /// <param name="optionalMessageId">If specified then this will be used for the service bus message id, if null or whitespace then an ID will be generated</param>
        /// <returns>The message ID</returns>
        Task<string> ScheduleEachMinuteCommand(ICommand<bool> command, DateTime initialExecutionDateTimeUtc, string optionalMessageId = null);

        /// <summary>
        /// Allows a command to be scheduled to be executed once a minute
        /// </summary>
        /// <param name="command">The command should return true if it wants to be rescheduled, false to stop.</param>
        /// <param name="optionalMessageId">If specified then this will be used for the service bus message id, if null or whitespace then an ID will be generated</param>
        /// <returns>The message ID</returns>
        Task<string> ScheduleEachMinuteCommand(ICommand<bool> command, string optionalMessageId = null);

        /// <summary>
        /// Cancels a scheduled command
        /// </summary>
        /// <param name="commandId"></param>
        /// <returns></returns>
        Task Cancel(string commandId);
    }
}
