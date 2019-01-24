using System;
using AccidentalFish.Commanding.ServiceBusScheduler.Abstractions;
using Xunit;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Tests.Services.Implementation.ScheduledDateCalculator
{
    public class NextScheduledAtUtc
    {
        private readonly ServiceBusScheduler.Services.Implementation.ScheduledDateCalculator _testSubject = new ServiceBusScheduler.Services.Implementation.ScheduledDateCalculator();

        [Fact]
        public void ShouldAddOnOneDayForDailyRecurrence()
        {
            DateTime now = DateTime.UtcNow;
            
            // Act
            DateTime result = _testSubject.NextScheduledAtUtc(now, ScheduledMessageRecurrenceEnum.Daily);

            // Assert
            Assert.Equal(now.AddDays(1), result);
        }

        [Fact]
        public void ShouldAddOnOneHourForHourlyRecurrence()
        {
            DateTime now = DateTime.UtcNow;

            // Act
            DateTime result = _testSubject.NextScheduledAtUtc(now, ScheduledMessageRecurrenceEnum.Hourly);

            // Assert
            Assert.Equal(now.AddHours(1), result);
        }

        [Fact]
        public void ShouldAddOnOneMinuteForMinutelyRecurrence()
        {
            DateTime now = DateTime.UtcNow;

            // Act
            DateTime result = _testSubject.NextScheduledAtUtc(now, ScheduledMessageRecurrenceEnum.Minute);

            // Assert
            Assert.Equal(now.AddMinutes(1), result);
        }

        [Fact]
        public void ShouldThrowExceptionForUnregisteredCalculator()
        {
            Assert.Throws<NotSupportedException>(() =>
                _testSubject.NextScheduledAtUtc(DateTime.UtcNow, ScheduledMessageRecurrenceEnum.DoesNotRecur));
        }
    }
}
