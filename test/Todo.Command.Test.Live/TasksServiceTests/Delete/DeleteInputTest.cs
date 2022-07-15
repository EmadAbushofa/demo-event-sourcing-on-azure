using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions.Persistence;
using Todo.Command.Events;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.Created;
using Todo.Command.Test.Helpers;
using Todo.Command.Test.Live.EventBus;
using Todo.Command.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.Live.TasksServiceTests.Delete
{
    public class DeleteInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public DeleteInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.AddServiceBusListener();
            });
        }

        [Fact]
        public async Task Delete_SendValidRequest_TaskDeletedEventSaved()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new DeleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var response = await client.DeleteAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(2, events.Count);
            AssertEquality.OfDeletedEvent(events[1], request, response, expectedSequence: 2);
        }

        [Fact]
        public async Task Delete_SendValidRequest_TaskDeletedEventPublished()
        {
            var listener = _factory.Services.GetRequiredService<TodoCommandListener>();

            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new DeleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var response = await client.DeleteAsync(request);

            listener.Events.Clear();

            await Task.Delay(8000);

            await listener.CloseAsync();

            Assert.Equal(2, listener.Events.Count);
            AssertEquality.OfDeletedEvent(listener.Events[1], request, response, expectedSequence: 2);
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