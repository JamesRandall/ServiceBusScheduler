using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AccidentalFish.Commanding.ServiceBusScheduler.Services;
using AccidentalFish.Commanding.ServiceBusScheduler.Tests.TestHelpers;
using Microsoft.Azure.ServiceBus;
using NSubstitute;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Services.Implementation.CommandScheduler
{
    public class ScheduleCommand
    {
        private readonly IQueueClientProvider _queueClientProvider = Substitute.For<IQueueClientProvider>();

        private readonly IQueueClient _queueClient = Substitute.For<IQueueClient>();

        private readonly ICancelledCommandRepository _cancelledCommandRepository =
            Substitute.For<ICancelledCommandRepository>();

        private readonly ServiceBusScheduler.Services.Implementation.CommandScheduler _testSubject;

        public ScheduleCommand()
        {
            _queueClientProvider.QueueClient.Returns(_queueClient);
            _testSubject = new ServiceBusScheduler.Services.Implementation.CommandScheduler(_queueClientProvider, _cancelledCommandRepository);
        }

        [Fact]
        public async Task ShouldThrowExceptionOnNullCommand()
        {
            // Act
            await Assert.ThrowsAsync<ArgumentNullException>(() => _testSubject.ScheduleCommand(ScheduledMessageRecurrenceEnum.Daily, null, null));
        }

        [Fact]
        public async Task ShouldThrowExceptionIfInitialDateLessThanNow()
        {
            // Arrange
            DateTime scheduleDate = DateTime.UtcNow.AddDays(-1);
            SimpleSchedulableCommand command = new SimpleSchedulableCommand();

            // Act
            await Assert.ThrowsAsync<ArgumentException>(() => _testSubject.ScheduleCommand(ScheduledMessageRecurrenceEnum.Daily, command, scheduleDate));
        }

        [Fact]
        public async Task ShouldPostMessageAtInitialExecutionDate()
        {
            // Arrange
            Message capturedMessage = null;
            DateTime scheduleDate = DateTime.UtcNow.AddDays(1);
            SimpleSchedulableCommand command = new SimpleSchedulableCommand();
            await _queueClient.SendAsync(Arg.Do<Message>(m => capturedMessage = m));

            // Act
            string messageId = await _testSubject.ScheduleCommand(ScheduledMessageRecurrenceEnum.Daily, command, scheduleDate);

            // Assert
            Assert.NotEmpty(messageId);
            Assert.Equal(scheduleDate, capturedMessage.ScheduledEnqueueTimeUtc);
            Assert.Equal(messageId, capturedMessage.MessageId);
        }

        [Fact]
        public async Task ShouldSetCustomMessageId()
        {
            // Arrange
            const string customMessageId = "1234";
            Message capturedMessage = null;
            SimpleSchedulableCommand command = new SimpleSchedulableCommand();
            await _queueClient.SendAsync(Arg.Do<Message>(m => capturedMessage = m));

            // Act
            string messageId = await _testSubject.ScheduleCommand(ScheduledMessageRecurrenceEnum.Daily, command, null, customMessageId);

            // Assert
            Assert.Equal(customMessageId, messageId);
            Assert.Equal(customMessageId, capturedMessage.MessageId);
        }
    }
}
