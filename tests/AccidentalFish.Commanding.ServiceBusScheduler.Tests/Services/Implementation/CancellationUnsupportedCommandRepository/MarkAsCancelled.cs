using System;
using System.Threading.Tasks;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Services.Implementation.CancellationUnsupportedCommandRepository
{
    public class MarkAsCancelled
    {
        private readonly ServiceBusScheduler.Services.Implementation.CancellationUnsupportedCommandRepository _testSubject =
            new ServiceBusScheduler.Services.Implementation.CancellationUnsupportedCommandRepository();

        [Fact]
        public async Task ShouldThrowException()
        {
            // Act
            var exception = await Assert.ThrowsAsync<NotSupportedException>(() => _testSubject.MarkAsCancelled("something"));
            Assert.Equal("To support cancellation register a persistent store for command IDs", exception.Message);
        }
    }
}
