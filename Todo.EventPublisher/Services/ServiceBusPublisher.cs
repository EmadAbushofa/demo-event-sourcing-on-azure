using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using Todo.EventPublisher.Entities;

namespace Todo.EventPublisher.Services
{
    public class ServiceBusPublisher
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusPublisher(ServiceBusClient client, ServiceBusPublisherOptions options)
        {
            _sender = client.CreateSender(
                queueOrTopicName: options.TopicName
            );
        }

        public Task PublishAsync(Event @event)
        {
            var json = JsonConvert.SerializeObject(@event);

            var body = Encoding.UTF8.GetBytes(json);

            var serviceBusMessage = new ServiceBusMessage(body)
            {
                PartitionKey = @event.AggregateId,
                SessionId = @event.AggregateId,
                Subject = @event.Type,
                ApplicationProperties = {
                                    { nameof(@event.AggregateId), @event.AggregateId },
                                    { nameof(@event.Type), @event.Type },
                                    { nameof(@event.Sequence), @event.Sequence },
                                    { nameof(@event.Version), @event.Version },
                                }
            };

            return _sender.SendMessageAsync(serviceBusMessage);
        }
    }
}
