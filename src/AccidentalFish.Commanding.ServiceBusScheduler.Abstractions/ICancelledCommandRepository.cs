using System.Threading.Tasks;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Abstractions
{
    /// <summary>
    /// The cancelled command repository manages a simple list of IDs to be cancelled
    /// </summary>
    public interface ICancelledCommandRepository
    {
        /// <summary>
        /// Marks a command as cancelled by storing the given ID
        /// </summary>
        Task MarkAsCancelled(string commandId);

        /// <summary>
        /// Tests to see if a command has been marked as cancelled
        /// </summary>
        Task<bool> IsMarkedAsCancelled(string commandId);

        /// <summary>
        /// Removes a cancellation flag if it has been stored, and returns whether or not it was stored 
        /// </summary>
        Task<bool> TryRemoveCancelFlag(string commandId);
    }
}
