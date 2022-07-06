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

namespace Todo.Command.Test.Live.TasksServiceTests.UpdateInfo
{
    public class UpdateInfoInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UpdateInfoInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.AddServiceBusListener();
            });
        }

        [Fact]
        public async Task UpdateInfo_SendValidRequest_TaskInfoUpdatedEventSaved()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new UpdateInfoRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                Title = "New title",
                Note = "New note",
            };

            var response = await client.UpdateInfoAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(2, events.Count);
            AssertEquality.OfInfoUpdatedEvent(events[1], request, response, expectedSequence: 2);
        }

        [Fact]
        public async Task UpdateInfo_SendValidRequest_TaskInfoUpdatedEventPublished()
        {
            var listener = _factory.Services.GetRequiredService<TodoCommandListener>();

            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new UpdateInfoRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                Title = "New title",
                Note = "New note",
            };

            var response = await client.UpdateInfoAsync(request);

            listener.Events.Clear();

            await Task.Delay(5000);

            await listener.CloseAsync();

            Assert.Equal(2, listener.Events.Count);
            AssertEquality.OfInfoUpdatedEvent(listener.Events[1], request, response, expectedSequence: 2);
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