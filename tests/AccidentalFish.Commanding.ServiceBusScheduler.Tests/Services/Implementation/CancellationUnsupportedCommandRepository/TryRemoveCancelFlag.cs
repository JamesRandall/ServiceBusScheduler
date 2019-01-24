using System.Threading.Tasks;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Services.Implementation.CancellationUnsupportedCommandRepository
{
    public class TryRemoveCancelFlag
    {
        private readonly ServiceBusScheduler.Services.Implementation.CancellationUnsupportedCommandRepository _testSubject =
            new ServiceBusScheduler.Services.Implementation.CancellationUnsupportedCommandRepository();

        [Fact]
        public async Task ShouldReturnFalse()
        {
            var result = await _testSubject.TryRemoveCancelFlag("someid");
            Assert.False(result);
        }
    }
}
