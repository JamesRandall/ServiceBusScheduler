using System;
using AzureFromTheTrenches.Commanding.Abstractions;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Abstractions
{
    /// <summary>
    /// A command that wraps the scheduled command and annotates it with scheduling information
    /// </summary>
    public class ScheduledMessage : ICommand
    {
        /// <summary>
        /// The ID of the message - same as the service bus message ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The recurrence type
        /// </summary>
        public ScheduledMessageRecurrenceEnum Recurrence { get; set; }

        /// <summary>
        /// The date and time the command was last scheduled
        /// </summary>
        public DateTime? LastScheduledAtUtc { get; set; }

        /// <summary>
        /// The date and time the command was last executed
        /// </summary>
        public DateTime? LastExecutedAtUtc { get; set; }

        /// <summary>
        /// The date and time the command is next scheduled to execute (should match the visibility property
        /// on the service bus message)
        /// </summary>
        public DateTime ScheduledAtUtc { get; set; }

        /// <summary>
        /// The FQN of the wrapped command type
        /// </summary>
        public string WrappedCommandFullTypeName { get; set; }

        /// <summary>
        /// The JSON that represents the wrapped command
        /// </summary>
        public string WrappedCommandJson { get;set; }
    }
}
