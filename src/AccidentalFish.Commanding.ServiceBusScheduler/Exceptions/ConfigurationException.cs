using System;

namespace AccidentalFish.Commanding.ServiceBusScheduler.Exceptions
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}
