namespace AccidentalFish.Commanding.ServiceBusScheduler
{
    /// <summary>
    /// Options for the scheduler
    /// </summary>
    public class Options
    {
        /// <summary>
        /// The service bus connection string
        /// </summary>
        public string ServiceBusConnectionString { get; set; }

        /// <summary>
        /// The name of the queue
        /// </summary>
        public string QueueName { get; set; }
    }
}
