using System;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Services.Implementation
{
    internal class CommandRescheduler : ICommandRescheduler
    {
        private readonly ICancelledCommandRepository _cancelledCommandRepository;
        private readonly IQueueClientProvider _queueClientProvider;
        private readonly IScheduledDateCalculator _dateCalculator;

        public CommandRescheduler(
            ICancelledCommandRepository cancelledCommandRepository,
            IQueueClientProvider queueClientProvider,
            IScheduledDateCalculator dateCalculator)
        {
            _cancelledCommandRepository = cancelledCommandRepository;
            _queueClientProvider = queueClientProvider;
            _dateCalculator = dateCalculator;
        }

        public async Task<bool> Reschedule(ScheduledMessage message)
        {
            bool isMarkedAsCancelled = await _cancelledCommandRepository.TryRemoveCancelFlag(message.Id);
            if (isMarkedAsCancelled)
            {
                return false;
            }

            if (message.Recurrence == ScheduledMessageRecurrenceEnum.DoesNotRecur)
            {
                return false;
            }

            DateTime newScheduledAtUtc = _dateCalculator.NextScheduledAtUtc(message.ScheduledAtUtc, message.Recurrence);

            ScheduledMessage newMessage = new ScheduledMessage
            {
                Id = message.Id,
                LastExecutedAtUtc = DateTime.UtcNow,
                LastScheduledAtUtc = message.ScheduledAtUtc,
                Recurrence = message.Recurrence,
                ScheduledAtUtc = newScheduledAtUtc,
                WrappedCommandFullTypeName = message.WrappedCommandFullTypeName,
                WrappedCommandJson = message.WrappedCommandJson
            };

            IQueueClient queueClient = _queueClientProvider.QueueClient;
            string json = JsonConvert.SerializeObject(newMessage);
            Message queueMessage = new Message(Encoding.UTF8.GetBytes(json));
            queueMessage.MessageId = newMessage.Id;
            queueMessage.ScheduledEnqueueTimeUtc = newScheduledAtUtc;
            await queueClient.SendAsync(queueMessage); // this should be the very last thing we do

            return true;
        }
    }
}
