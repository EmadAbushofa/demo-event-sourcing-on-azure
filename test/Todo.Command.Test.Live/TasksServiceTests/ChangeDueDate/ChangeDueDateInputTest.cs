using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions;
using Todo.Command.Events;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.Created;
using Todo.Command.Test.Helpers;
using Todo.Command.Test.Live.EventBus;
using Todo.Command.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.Live.TasksServiceTests.ChangeDueDate
{
    public class ChangeDueDateInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ChangeDueDateInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.AddServiceBusListener();
            });
        }

        [Fact]
        public async Task ChangeDueDate_SendValidRequest_TaskDueDateChangedEventSaved()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new ChangeDueDateRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
            };

            var response = await client.ChangeDueDateAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(2, events.Count);
            AssertEquality.OfDueDateChangedEvent(events[1], request, response, expectedSequence: 2);
        }

        [Fact]
        public async Task ChangeDueDate_SendValidRequest_TaskDueDateChangedEventPublished()
        {
            var listener = _factory.Services.GetRequiredService<TodoCommandListener>();

            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new ChangeDueDateRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
            };

            var response = await client.ChangeDueDateAsync(request);

            listener.Events.Clear();

            await Task.Delay(5000);

            await listener.CloseAsync();

            Assert.Equal(2, listener.Events.Count);
            AssertEquality.OfDueDateChangedEvent(listener.Events[1], request, response, expectedSequence: 2);
        }

        [Fact]
        public async Task ChangeDueDate_SendValidRequestMultipleTimes_TaskDueDateChangedEventSavedOnce()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new ChangeDueDateRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
            };

            Task<Response> SendAsync() => client.ChangeDueDateAsync(request).ResponseAsync;

            var responses = await Task.WhenAll(
                SendAsync(),
                SendAsync(),
                SendAsync()
            );

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(createdEvent.AggregateId);

            Assert.All(responses, r => Assert.NotEmpty(r.Id));
            Assert.Equal(2, events.Count);
            AssertEquality.OfDueDateChangedEvent(events[1], request, responses[1], expectedSequence: 2);
        }

        private async Task<TaskCreated> GenerateAndAppendToStreamAsync()
        {
            var eventStore = _factory.Services.GetRequiredService<IEventStore>();

            var taskCreatedEvent = new TaskCreatedFaker()
                .Generate();

            await eventStore.AppendToStreamAsync(taskCreatedEvent);

            return taskCreatedEvent;
        }
    }
}