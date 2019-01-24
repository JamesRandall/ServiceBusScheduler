using System;
using System.Linq;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AccidentalFish.Commanding.ServiceBusScheduler.Exceptions;
using AccidentalFish.Commanding.ServiceBusScheduler.Services;
using AzureFromTheTrenches.Commanding.Abstractions;
using Newtonsoft.Json;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Handlers
{
    internal class ScheduledMessageHandler<TDispatcherType> : ICommandHandler<ScheduledMessage> where TDispatcherType : IFrameworkCommandDispatcher
    {
        private readonly IRegistrationCatalogue _registry;
        private readonly ICommandRescheduler _commandRescheduler;
        private readonly TDispatcherType _dispatcher;

        public ScheduledMessageHandler(ICommandRegistry registry,
            ICommandRescheduler commandRescheduler,
            TDispatcherType dispatcher)
        {
            // NOTE: I need to update the commanding / mediator framework to register the catalogue interface
            // The below cast is a bit of a nasty workaround until I can.
            _registry = (IRegistrationCatalogue)registry;
            _commandRescheduler = commandRescheduler;
            _dispatcher = dispatcher;
        }

        public async Task ExecuteAsync(ScheduledMessage scheduledMessage)
        {
            Type wrappedCommandType = _registry.GetRegisteredCommands()
                .FirstOrDefault(x => x.FullName == scheduledMessage.WrappedCommandFullTypeName);
            if (wrappedCommandType == null)
            {
                throw new ConfigurationException($"Command type {scheduledMessage.WrappedCommandFullTypeName} has not been registered");
            }

            ICommand<bool> wrappedCommand = (ICommand<bool>)JsonConvert.DeserializeObject(scheduledMessage.WrappedCommandJson, wrappedCommandType);
            bool shouldReschedule = await _dispatcher.DispatchAsync(wrappedCommand);

            if (shouldReschedule)
            {
                // this should be the very last thing we do - otherwise an exception may cause a double queue of
                // the rescheduled message
                await _commandRescheduler.Reschedule(scheduledMessage); 
            }

            // there is a chance that between here and this message being removed from the queue
            // that an exception occurs meaning the message will be popped and rescheduled again
            // leading to duplication
            // to protect against this the message ID is set on the service bus message and Service Bus
            // Deduplication should be turned on, consider the message ID time window based on the
            // recurrence interval - do not set the deuplication window to be larger than the interval
            // otherwise legitimate reschedules will be detected as duplicates by SB.
        }
    }
}
