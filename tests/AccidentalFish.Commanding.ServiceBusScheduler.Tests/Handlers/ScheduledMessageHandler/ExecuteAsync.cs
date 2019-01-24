using System;
using System.Threading.Tasks;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using AccidentalFish.Commanding.ServiceBusScheduler.Exceptions;
using AccidentalFish.Commanding.ServiceBusScheduler.Handlers;
using AccidentalFish.Commanding.ServiceBusScheduler.Tests.TestHelpers;
using AzureFromTheTrenches.Commanding.Abstractions;
using AzureFromTheTrenches.Commanding.Abstractions.Model;
using NSubstitute;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Handlers.ScheduledMessageHandler
{
    public class ExecuteAsync
    {
        private readonly ICommandRegistry _commandRegistry = Substitute.For<ICommandRegistry, IRegistrationCatalogue>();
        private readonly IRegistrationCatalogue _registrationCatalogue;
        private readonly ICommandRescheduler _commandRescheduler = Substitute.For<ICommandRescheduler>();
        private readonly ICommandDispatcher _commandDispatcher = Substitute.For<ICommandDispatcher>();

        private readonly ScheduledMessageHandler<ICommandDispatcher> _testSubject;

        public ExecuteAsync()
        {
            _registrationCatalogue = (IRegistrationCatalogue)_commandRegistry;
            _testSubject = new ScheduledMessageHandler<ICommandDispatcher>(
                _commandRegistry,
                _commandRescheduler,
                _commandDispatcher);
        }

        [Fact]
        public async Task ShouldThrowConfigurationExceptionWhenCommandNotFound()
        {
            // Arrange
            _registrationCatalogue.GetRegisteredCommands().Returns(new Type[0]);

            // Act
            var exception = await Assert.ThrowsAsync<ConfigurationException>(() =>
                _testSubject.ExecuteAsync(new SimpleSchedulableCommand().CreateScheduledMessage()));
            Assert.Equal($"Command type {typeof(SimpleSchedulableCommand).FullName} has not been registered", exception.Message);
        }

        [Fact]
        public async Task ShouldNotRecheduleCommandThatReturnsFalse()
        {
            // Arrange
            _registrationCatalogue.GetRegisteredCommands().Returns(new Type[] { typeof(SimpleSchedulableCommand) });
            SimpleSchedulableCommand command = new SimpleSchedulableCommand();
            _commandDispatcher.DispatchAsync(command).ReturnsForAnyArgs(new CommandResult<bool>(false, false));

            // Act
            await _testSubject.ExecuteAsync(new SimpleSchedulableCommand().CreateScheduledMessage());

            // Assert
            await _commandRescheduler.DidNotReceiveWithAnyArgs().Reschedule(null);
        }

        [Fact]
        public async Task ShouldRecheduleCommandThatReturnsTrue()
        {
            // Arrange
            _registrationCatalogue.GetRegisteredCommands().Returns(new Type[] { typeof(SimpleSchedulableCommand) });
            SimpleSchedulableCommand command = new SimpleSchedulableCommand();
            _commandDispatcher.DispatchAsync(command).ReturnsForAnyArgs(new CommandResult<bool>(true, false));

            // Act
            await _testSubject.ExecuteAsync(new SimpleSchedulableCommand().CreateScheduledMessage());

            // Assert
            await _commandRescheduler.ReceivedWithAnyArgs().Reschedule(null);
        }
    }
}
