﻿using Azure.Messaging.ServiceBus;
using MediatR;
using Newtonsoft.Json;
using System.Text;
using Todo.Query.Enums;
using Todo.Query.Features.Create;

namespace Todo.Query.Infrastructure.ServiceBus
{
    public class TodoListener : IHostedService
    {
        private readonly ServiceBusSessionProcessor _processor;
        private readonly IServiceProvider _provider;
        private readonly ILogger<TodoListener> _logger;

        public TodoListener(
            IServiceProvider provider,
            IConfiguration configuration,
            ILogger<TodoListener> logger,
            ServiceBusClient client
        )
        {
            var options = configuration.GetSection(ServiceBusOptions.ServiceBus).Get<ServiceBusOptions>();

            _processor = client.CreateSessionProcessor(
                topicName: options.TopicName,
                subscriptionName: options.SubscriptionName,
                new ServiceBusSessionProcessorOptions()
                {
                    AutoCompleteMessages = false,
                    MaxConcurrentCallsPerSession = 1,
                    MaxConcurrentSessions = 1000,
                    PrefetchCount = 1,
                    ReceiveMode = ServiceBusReceiveMode.PeekLock
                }
            );

            _processor.ProcessMessageAsync += Processor_ProcessMessageAsync;
            _processor.ProcessErrorAsync += Processor_ProcessErrorAsync;
            _provider = provider;
            _logger = logger;
        }

        private async Task Processor_ProcessMessageAsync(ProcessSessionMessageEventArgs arg)
        {
            var isHandled = await TryHandleAsync(arg.Message);

            if (isHandled)
                await arg.CompleteMessageAsync(arg.Message);
        }

        private async Task<bool> TryHandleAsync(ServiceBusReceivedMessage message)
        {
            _logger.LogInformation(
                "Event {Event} Arrived, SessionId {SessionId}.",
                message.Subject,
                message.SessionId
            );

            using var scope = _provider.CreateScope();

            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var json = Encoding.UTF8.GetString(message.Body);

            return message.Subject switch
            {
                nameof(EventType.TaskCreated) => await mediator.Send(Deserialize<TaskCreatedEvent>(json)),
                _ => true,
            };
        }

        private static TEvent Deserialize<TEvent>(string json)
            => JsonConvert.DeserializeObject<TEvent>(json)
            ?? throw new ArgumentException("Message deserialization failed", nameof(json));

        private Task Processor_ProcessErrorAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogCritical(arg.Exception, "Message handler encountered an exception," +
                " Error Source:{ErrorSource}," +
                " Entity Path:{EntityPath}",
                arg.ErrorSource.ToString(),
                arg.EntityPath
            );

            return Task.CompletedTask;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _processor.StartProcessingAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _processor.CloseAsync(cancellationToken);
        }
    }
}