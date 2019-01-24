using System.Linq;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AccidentalFish.Commanding.ServiceBusScheduler.Handlers;
using AccidentalFish.Commanding.ServiceBusScheduler.Services;
using AccidentalFish.Commanding.ServiceBusScheduler.Services.Implementation;
using AzureFromTheTrenches.Commanding.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace AccidentalFish.Commanding.ServiceBusScheduler
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection UseServiceBusScheduler(
            this IServiceCollection serviceCollection,
            ICommandRegistry commandRegistry,
            Options options)
        {
            return UseServiceBusScheduler<ICommandDispatcher>(serviceCollection, commandRegistry, options);
        }

        public static IServiceCollection UseServiceBusScheduler<TCommandDispatcherType>(
            this IServiceCollection serviceCollection,
            ICommandRegistry commandRegistry,
            Options options) where TCommandDispatcherType : IFrameworkCommandDispatcher
        {
            serviceCollection
                .AddSingleton<IQueueClientProvider>(new QueueClientProvider(options.ServiceBusConnectionString, options.QueueName))
                .AddTransient<ICommandRescheduler, CommandRescheduler>()
                .AddTransient<ICommandScheduler, CommandScheduler>()
                .AddTransient<IScheduledDateCalculator, ScheduledDateCalculator>()
                ;

            if (serviceCollection.All(x => x.ServiceType != typeof(ICancelledCommandRepository)))
            {
                serviceCollection.AddTransient<ICancelledCommandRepository, CancellationUnsupportedCommandRepository>();
            }

            commandRegistry.Register<ScheduledMessageHandler<TCommandDispatcherType>>();
            return serviceCollection;
        }
    }
}
