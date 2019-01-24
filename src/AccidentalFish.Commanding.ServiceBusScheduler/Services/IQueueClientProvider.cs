using Microsoft.Azure.ServiceBus;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Services
{
    internal interface IQueueClientProvider
    {
        IQueueClient QueueClient { get; }
    }
}
