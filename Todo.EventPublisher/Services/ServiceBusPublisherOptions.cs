using Microsoft.Extensions.Configuration;

namespace Todo.EventPublisher.Services
{
    public class ServiceBusPublisherOptions
    {
        public ServiceBusPublisherOptions(IConfiguration configuration)
        {
            ConnectionString = configuration["ServiceBusConnection"];
            TopicName = configuration["Collection"];
        }

        public string ConnectionString { get; }
        public string TopicName { get; }
    }
}
