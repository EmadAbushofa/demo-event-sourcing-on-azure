using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using Todo.Command.Events;
using Todo.Command.Infrastructure.Persistence;

namespace Todo.Command.Test.Live.EventBus
{
    public class TodoCommandListener
    {
        private readonly ServiceBusSessionProcessor _processor;

        public TodoCommandListener(ServiceBusClient client, TodoCommandListenerOptions options)
        {
            _processor = client.CreateSessionProcessor(
                topicName: options.TopicName,
                subscriptionName: options.SubscriptionName,
                options: new ServiceBusSessionProcessorOptions()
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentSessions = 1000,
                    MaxConcurrentCallsPerSession = 1,
                    PrefetchCount = 1,
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                }
            );

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            _processor.StartProcessingAsync();
        }

        public List<Event> Events = new();

        private Task Processor_ProcessMessageAsync(ProcessSessionMessageEventArgs arg)
        {
            var json = Encoding.UTF8.GetString(arg.Message.Body);

            var @event = JsonConvert.DeserializeObject<Document>(json).ToEvent();

            Events.Add(@event);

            return arg.CompleteMessageAsync(arg.Message);
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg) => throw arg.Exception;

        public Task CloseAsync() => _processor.CloseAsync();
    }
}
