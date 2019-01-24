using System.Threading.Tasks;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Abstractions
{
    /// <summary>
    /// Is responsible for rescheduling commands
    /// </summary>
    public interface ICommandRescheduler
    {
        /// <summary>
        /// Reschedules the given message
        /// </summary>
        Task<bool> Reschedule(ScheduledMessage message);
    }
}
