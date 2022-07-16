using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Todo.Query.Abstractions;
using Todo.Query.Infrastructure.Abstractions.MessageObjects;

namespace Todo.Query.Infrastructure.ServiceBus
{
    public class ServiceBusPublisher : IMessagePublisher
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusPublisher(ServiceBusClient serviceBusClient, IConfiguration configuration)
        {
            var options = configuration.GetSection(ServiceBusOptions.ServiceBus).Get<ServiceBusOptions>();

            _sender = serviceBusClient.CreateSender(options.QueryTopicName);
        }

        public void Send(EventConsumedMessage message)
        {
            if (message.Event == null)
                throw new InvalidOperationException("Event is null in message");

            if (message.Entity == null)
                throw new InvalidOperationException("Entity is null in message");

            var json = JsonConvert.SerializeObject(message);

            var body = Encoding.UTF8.GetBytes(json);

            var serviceBusMessage = new ServiceBusMessage(body)
            {
                PartitionKey = message.Event.AggregateId.ToString(),
                SessionId = message.Event.AggregateId.ToString(),
                Subject = message.Type,
                ApplicationProperties = {
                                    { nameof(message.Event.AggregateId), message.Event.AggregateId },
                                    { nameof(message.Type), message.Type },
                                    { nameof(message.Event.Sequence), message.Event.Sequence },
                                    { nameof(message.Event.Version), message.Event.Version },
                                }
            };

            _sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
