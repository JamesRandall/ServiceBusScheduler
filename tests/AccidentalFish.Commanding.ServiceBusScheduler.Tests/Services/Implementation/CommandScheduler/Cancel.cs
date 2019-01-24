using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AccidentalFish.Commanding.ServiceBusScheduler.Services;
using Microsoft.Azure.ServiceBus;
using NSubstitute;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Services.Implementation.CommandScheduler
{
    public class Cancel
    {
        private readonly IQueueClientProvider _queueClientProvider = Substitute.For<IQueueClientProvider>();

        private readonly IQueueClient _queueClient = Substitute.For<IQueueClient>();

        private readonly ICancelledCommandRepository _cancelledCommandRepository =
            Substitute.For<ICancelledCommandRepository>();

        private readonly ServiceBusScheduler.Services.Implementation.CommandScheduler _testSubject;

        public Cancel()
        {
            _queueClientProvider.QueueClient.Returns(_queueClient);
            _testSubject = new ServiceBusScheduler.Services.Implementation.CommandScheduler(_queueClientProvider, _cancelledCommandRepository);
        }

        [Fact]
        public async Task ShouldThrowExceptionOnNullCommandId()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _testSubject.Cancel(null));
        }

        [Fact]
        public async Task ShouldMarkAsCancelled()
        {
            // Act
            await _testSubject.Cancel("someid");

            // Assert
            await _cancelledCommandRepository.Received().MarkAsCancelled("someid");
        }
    }
}
