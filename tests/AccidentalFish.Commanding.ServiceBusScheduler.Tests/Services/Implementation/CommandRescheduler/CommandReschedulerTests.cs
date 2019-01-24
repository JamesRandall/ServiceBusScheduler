using System;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AccidentalFish.Commanding.ServiceBusScheduler.Services;
using AccidentalFish.Commanding.ServiceBusScheduler.Tests.TestHelpers;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Services.Implementation.CommandRescheduler
{
    public class Reschedule
    {
        private readonly ICancelledCommandRepository _cancelledCommandRepository =
            Substitute.For<ICancelledCommandRepository>();

        private readonly IQueueClientProvider _queueClientProvider =
            Substitute.For<IQueueClientProvider>();

        private readonly IQueueClient _queueClient =
            Substitute.For<IQueueClient>();

        private readonly IScheduledDateCalculator _dateCalculator =
            Substitute.For<IScheduledDateCalculator>();

        private readonly ServiceBusScheduler.Services.Implementation.CommandRescheduler _testSubject;

        public Reschedule()
        {
            _testSubject = new ServiceBusScheduler.Services.Implementation.CommandRescheduler(_cancelledCommandRepository, _queueClientProvider, _dateCalculator);
            _queueClientProvider.QueueClient.Returns(_queueClient);
        }

        [Fact]
        public async Task ShouldNotRequeueWhenCommandMarkedAsCancelled()
        {
            // Arrange
            const string testId = "1234";
            ScheduledMessage message = new SimpleSchedulableCommand().CreateScheduledMessage(testId);
            _cancelledCommandRepository.TryRemoveCancelFlag(testId).Returns(true);

            // Act
            bool didReschedule = await _testSubject.Reschedule(message);

            // Assert
            Assert.False(didReschedule);
            await _queueClient.DidNotReceiveWithAnyArgs().SendAsync(new Message());
        }

        [Fact]
        public async Task ShouldQueueNewMessageWhenRescheduleRequired()
        {
            // Arrange
            DateTime newScheduleDateTimeUtc = new DateTime(2019, 8, 1, 17, 31, 0);
            const string testId = "1234";
            ScheduledMessage message = new SimpleSchedulableCommand().CreateScheduledMessage(testId);
            _cancelledCommandRepository.TryRemoveCancelFlag(testId).Returns(false);
            _dateCalculator.NextScheduledAtUtc(DateTime.UtcNow, ScheduledMessageRecurrenceEnum.Daily)
                .ReturnsForAnyArgs(newScheduleDateTimeUtc);
            Message sentServiceBusMessage = null;
            await _queueClient.SendAsync(Arg.Do<Message>(x => sentServiceBusMessage = x));

            // Act
            bool didReschedule = await _testSubject.Reschedule(message);
            ScheduledMessage sentMessage =
                JsonConvert.DeserializeObject<ScheduledMessage>(Encoding.UTF8.GetString(sentServiceBusMessage.Body));

            // Assert
            Assert.Equal(testId, sentServiceBusMessage.MessageId);
            Assert.Equal(newScheduleDateTimeUtc, sentServiceBusMessage.ScheduledEnqueueTimeUtc);
            
            Assert.Equal(message.Id, sentMessage.Id);
            Assert.Equal(message.Recurrence, sentMessage.Recurrence);
            Assert.Equal(message.ScheduledAtUtc, sentMessage.LastScheduledAtUtc);
            Assert.Equal(newScheduleDateTimeUtc, sentMessage.ScheduledAtUtc);
            Assert.Equal(message.WrappedCommandFullTypeName, sentMessage.WrappedCommandFullTypeName);
            Assert.Equal(message.WrappedCommandJson, sentMessage.WrappedCommandJson);
        }
    }
}
