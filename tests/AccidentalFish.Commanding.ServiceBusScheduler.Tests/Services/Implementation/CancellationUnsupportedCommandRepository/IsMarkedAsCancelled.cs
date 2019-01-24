using System.Threading.Tasks;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Services.Implementation.CancellationUnsupportedCommandRepository
{
    public class IsMarkedAsCancelled
    {
        private readonly ServiceBusScheduler.Services.Implementation.CancellationUnsupportedCommandRepository _testSubject =
            new ServiceBusScheduler.Services.Implementation.CancellationUnsupportedCommandRepository();

        [Fact]
        public async Task ShouldReturnFalse()
        {
            var result = await _testSubject.IsMarkedAsCancelled("someid");
            Assert.False(result);
        }
    }
}
