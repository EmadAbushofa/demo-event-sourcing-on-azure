using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Helpers;
using Todo.Command.Test.Live.EventBus;
using Todo.Command.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.Live.TasksServiceTests.Create
{
    public class CreateTaskTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.DisableQueryDuplicateDetection();
                services.AddServiceBusListener();
            });
        }

        [Fact]
        public async Task Create_SendValidRequest_TaskCreatedEventSaved()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Read a book",
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
                Note = "Domain Driven Design",
            };

            var response = await client.CreateAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Single(events);
            AssertEquality.OfCreatedEvent(events[0], request, response);
        }

        [Fact]
        public async Task Create_SendValidRequest_TaskCreatedEventPublished()
        {
            var listener = _factory.Services.GetRequiredService<TodoCommandListener>();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Read a book",
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
                Note = "Domain Driven Design",
            };

            var response = await client.CreateAsync(request);

            listener.Events.Clear();

            await Task.Delay(5000);

            await listener.CloseAsync();

            Assert.Single(listener.Events);
            AssertEquality.OfCreatedEvent(listener.Events[0], request, response);
        }
    }
}