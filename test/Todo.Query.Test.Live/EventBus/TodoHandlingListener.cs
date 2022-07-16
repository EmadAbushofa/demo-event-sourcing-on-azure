using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Todo.Query.Test.Live.EventBus
{
    public class TodoHandlingListener
    {
        private readonly ServiceBusProcessor _processor;

        public TodoHandlingListener(ServiceBusClient client, TodoHandlingListenerOptions options)
        {
            _processor = client.CreateProcessor(
                topicName: options.QueryTopicName,
                subscriptionName: options.QuerySubscriptionName,
                options: new ServiceBusProcessorOptions()
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCalls = 1000,
                    PrefetchCount = 1,
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                }
            );

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;

            _processor.StartProcessingAsync();
        }

        public List<ReceivedEventConsumedMessage> Messages = new();

        private Task Processor_ProcessMessageAsync(ProcessMessageEventArgs arg)
        {
            var json = Encoding.UTF8.GetString(arg.Message.Body);

            var message = JsonConvert.DeserializeObject<ReceivedEventConsumedMessage>(json)
                ?? throw new JsonSerializationException("Deserialization failed.");

            Messages.Add(message);

            return arg.CompleteMessageAsync(arg.Message);
        }

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg) => throw arg.Exception;

        public Task CloseAsync() => _processor.CloseAsync();
    }
}
