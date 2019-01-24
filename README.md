# Service Bus Scheduler

![Master build status](https://accidentalfish.visualstudio.com/Service%20Bus%20Scheduler/_apis/build/status/Service%20Bus%20Scheduler-ASP.NET%20Core-CI?branchName=master) Master

![Production build status](https://accidentalfish.visualstudio.com/Service%20Bus%20Scheduler/_apis/build/status/Service%20Bus%20Scheduler-ASP.NET%20Core-CI?branchName=production) Production

This library works in conjunction with the mediator / commanding framework [AzureFromTheTrenches.Commanding](https://commanding.azurefromthetrenches.com/) to provide command scheduling capabilities via the Azure Service Bus and supports automatically recurring events.

The use of the Service Bus makes it easy, cheap and convenient to dynamically generate scheduled events based on (for example) user input but has the drawback of being imprecise (you control the visibility on the queue so a busy queue and a slow backlogged processor could lead to a delay).

This documentation will assume you have a basic knowledge of the commanding framework.

## Getting Started

First add the package AccidentalFish.Commanding.ServiceBusScheduler:

    Install-Package AccidentalFish.Commanding.ServiceBusScheduler

Now assuming you have soem form of startup block for your code with access to an IServiceCollection add a block similar to the below to register the components:
    
    IServiceCollection serviceCollection = ...;
    ICommandRegistry commandRegistry = ...;
    
    serviceCollection.UseServiceBusScheduler(commandRegistry, new Options {
        ServiceBusConnectionString = "your service bus connection string",
        QueueName = "schedulerqueue"
    });

This will register the _ICommandScheduler_ interface which is the interface that is used to schedule commands. For example:

    class MyScheduler
    {
        private readonly ICommandScheduler _commandScheduler;

        public MyScheduler(ICommandScheduler commandScheduler)
        {
            _commandScheduler = commandScheduler;
        }

        public async Task ScheduleANotification(string message)
        {
            await _commandScheduler.ScheduleCommand(
                ScheduledMessageRecurrenceEnum.DoesNotRecur
                new Notification {
                    Message = message
                },
                DateTime.UtcNow.AddDays(1)
            );
        }        
    }

Commands used by the scheduler must implement the interface ICommand<bool> and their handler should return true if the command should be rescheduled or false if not. For non-recurring commands the return value is irrelavant. For example:

    class Notification : ICommand<bool>
    {
        public string Message { get; set; }
    }

    class NotificationHandler : ICommandHandler<Notification, bool>
    {
        public Task<bool> ExecuteAsync(Notification notification, bool previousResult)
        {
            // send the message
            ...

            return false; // do not recur
        }
    }

You will need to set up a listener for Service Bus messages that invokes the commands. One option is to use Azure Functions and the [FunctionMonkey](https://functionmonkey.azurefromthetrenches.como) package in which case add the AccidentalFish.Commanding.ServiceBusScheduler.FunctionMonkey package to your Azure Functions and Function Monkey project:

   Install-Package AccidentalFish.Commanding.ServiceBusScheduler.FunctionMonkey

You can then register a listener in the Service Bus section as shown in the below example:

    public class FunctionAppConfiguration : IFunctionAppConfiguration
    {
        private const string QueueName = "schedulerqueue";
        private const string ServiceBusConnectionName = "serviceBusConnection";

        public void Build(IFunctionHostBuilder builder)
        {
            builder
                .Setup((serviceCollection, commandRegistry) =>
                    commandRegistry.Register<SendEmailCommandHandler>()
                )
                .Functions(functions => functions
                    .ServiceBus(ServiceBusConnectionName, serviceBus => serviceBus
                        .ScheduledMessageReceiver(QueueName)
                    )
                );
        }
    }

You can also use the [Service Bus extensions for the mediator](https://commanding.azurefromthetrenches.com/guides/azureServiceBus/queueQuickstart.html#executing-queued-commands-from-queues-and-subscriptions) in conjunction with the schedulers own command type _ScheduledMessage_:

    QueueClient client = new QueueClient(serviceBusConnectionString, "myqueue");
    IServiceBusCommandQueueProcessor commandQueueProcessor = factory.Create<ScheduledMessage>(client);

Essentially you need to read messages of type _ScheduledMessage_ from the Service Bus and dispatch them through the _ICommandDispatcher_.

## Duplicates and Message IDs

For recurring events and depending on how your code is structured there is a small chance of a duplicate message being placed on the Service if there is an exception at the very end of your handling code - essentially after the ServiceBusScheduler package has posted the message for the next event but before your listener has told the Service Bus to delete the current message.

An easy way to mitigate this is to turn duplicate detection on on the Service Bus.

## Issues and Feedback

I've spun this framework out of another project and the docs could certainly use a little more work and there is a little more to cover around cancellation however hopefully it is useful.

If you come across any issues then please reach out to me on [Twitter](https://twitter.com/AzureTrenches) or use the GitHub Issues here.