using System;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using FunctionMonkey.Abstractions.Builders;

namespace AccidentalFish.Commanding.ServiceBusScheduler.FunctionMonkey
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceBusFunctionBuilderExtensions
    {
        public static IServiceBusFunctionBuilder ScheduledMessageReceiver(this IServiceBusFunctionBuilder functionBuilder, string queueName)
        {
            functionBuilder.QueueFunction<ScheduledMessage>(queueName);
            return functionBuilder;
        }
    }
}
