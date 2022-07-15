using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions.Persistence;
using Todo.Command.Events;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.Completed;
using Todo.Command.Test.Fakers.Created;
using Todo.Command.Test.Helpers;
using Todo.Command.Test.Live.EventBus;
using Todo.Command.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.Live.TasksServiceTests.Uncomplete
{
    public class UncompleteInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UncompleteInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.AddServiceBusListener();
            });
        }

        [Fact]
        public async Task Uncomplete_SendValidRequest_TaskUncompletedEventSaved()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CompleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var response = await client.UncompleteAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(3, events.Count);
            AssertEquality.OfUncompletedEvent(events[2], request, response, expectedSequence: 3);
        }

        [Fact]
        public async Task Uncomplete_SendValidRequest_TaskUncompletedEventPublished()
        {
            var listener = _factory.Services.GetRequiredService<TodoCommandListener>();

            var createdEvent = await GenerateAndAppendToStreamAsync();

            listener.Events.Clear();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CompleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var response = await client.UncompleteAsync(request);

            await Task.Delay(8000);

            await listener.CloseAsync();

            Assert.Equal(3, listener.Events.Count);
            AssertEquality.OfUncompletedEvent(listener.Events[2], request, response, expectedSequence: 3);
        }

        private async Task<TaskCreated> GenerateAndAppendToStreamAsync()
        {
            var eventStore = _factory.Services.GetRequiredService<IEventStore>();

            var taskCreated = new TaskCreatedFaker()
                .Generate();

            await eventStore.AppendToStreamAsync(taskCreated);

            var taskCompleted = new TaskCompletedFaker()
                .For(taskCreated)
                .Generate();

            await eventStore.AppendToStreamAsync(taskCompleted);

            return taskCreated;
        }
    }
}