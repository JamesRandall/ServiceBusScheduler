using System;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Services.Implementation
{
    internal class CommandScheduler : ICommandScheduler
    {
        private readonly IQueueClientProvider _queueClientProvider;
        private readonly ICancelledCommandRepository _cancelledCommandRepository;

        public CommandScheduler(IQueueClientProvider queueClientProvider,
            ICancelledCommandRepository cancelledCommandRepository)
        {
            _queueClientProvider = queueClientProvider;
            _cancelledCommandRepository = cancelledCommandRepository;
        }        

        public async Task Cancel(string commandId)
        {
            if (string.IsNullOrWhiteSpace(commandId)) throw new ArgumentNullException(nameof(commandId));
            await _cancelledCommandRepository.MarkAsCancelled(commandId);
        }

        public async Task<string> ScheduleCommand(ScheduledMessageRecurrenceEnum frequency, ICommand<bool> command, DateTime? initialExecutionDateTimeUtc, string optionalMessageId = null)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));
            if (initialExecutionDateTimeUtc.HasValue && initialExecutionDateTimeUtc < DateTime.UtcNow)
            {
                throw new ArgumentException("Commands cannot be scheduled to run in the past");
            }

            if (frequency == ScheduledMessageRecurrenceEnum.DoesNotRecur && !initialExecutionDateTimeUtc.HasValue)
            {
                throw new ArgumentException("A one off command must have an execution date", nameof(initialExecutionDateTimeUtc));
            }

            ScheduledMessage message = CreateScheduledMessage(command, initialExecutionDateTimeUtc, optionalMessageId, frequency);

            await PostMessage(initialExecutionDateTimeUtc, message);

            return message.Id;
        }

        private static ScheduledMessage CreateScheduledMessage(ICommand<bool> command,
            DateTime? initialExecutionDateTimeUtc,
            string optionalMessageId, ScheduledMessageRecurrenceEnum frequency)
        {
            DateTime scheduledAt = initialExecutionDateTimeUtc ?? DateTime.UtcNow;
            string messageId = string.IsNullOrWhiteSpace(optionalMessageId) ? Guid.NewGuid().ToString() : optionalMessageId;
            string wrappedCommandJson = JsonConvert.SerializeObject(command);
            return new ScheduledMessage
            {
                Id = messageId,
                LastScheduledAtUtc = null,
                LastExecutedAtUtc = null,
                Recurrence = frequency,
                ScheduledAtUtc = scheduledAt,
                WrappedCommandFullTypeName = command.GetType().FullName,
                WrappedCommandJson = wrappedCommandJson
            };
        }

        private async Task PostMessage(DateTime? initialExecutionDateTimeUtc, ScheduledMessage message)
        {
            IQueueClient queueClient = _queueClientProvider.QueueClient;
            string json = JsonConvert.SerializeObject(message);
            Message queueMessage = new Message(Encoding.UTF8.GetBytes(json));
            queueMessage.MessageId = message.Id;
            if (initialExecutionDateTimeUtc.HasValue)
            {
                queueMessage.ScheduledEnqueueTimeUtc = initialExecutionDateTimeUtc.Value;
            }

            await queueClient.SendAsync(queueMessage);
        }
    }
}
