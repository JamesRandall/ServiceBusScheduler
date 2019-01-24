using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Services.Implementation
{
    internal class CancellationUnsupportedCommandRepository : ICancelledCommandRepository
    {
        public Task MarkAsCancelled(string commandId)
        {
            throw new NotSupportedException("To support cancellation register a persistent store for command IDs");
        }

        public Task<bool> IsMarkedAsCancelled(string commandId)
        {
            return Task.FromResult(false);
        }

        public Task<bool> TryRemoveCancelFlag(string commandId)
        {
            return Task.FromResult(false);
        }
    }
}
