using Microsoft.Azure.ServiceBus;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Services.Implementation
{
    internal class QueueClientProvider : IQueueClientProvider
    {
        public QueueClientProvider(string serviceBusConnectionString, string queueName)
        {
            QueueClient = new QueueClient(serviceBusConnectionString, queueName);
        }

        public IQueueClient QueueClient { get; }
    }
}
