using AzureFromTheTrenches.Commanding.Abstractions;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.TestHelpers
{
    class SimpleSchedulableCommand : ICommand<bool>
    {
        public string Message { get; set; }
    }
}
