namespace Todo.Query.Infrastructure.ServiceBus
{
    public class ServiceBusOptions
    {
        public const string ServiceBus = "ServiceBus";
        public string? TopicName { get; set; }
        public string? SubscriptionName { get; set; }
        public string? QueryTopicName { get; set; }
    }
}
