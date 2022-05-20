using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstraction;
using Todo.Command.Test.Helpers;
using Todo.Command.TodoProto;
using Xunit.Abstractions;

namespace Todo.Command.Test.Live.TasksService
{
    public class CreateTaskTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
        }


        [Fact]
        public async void Create_SendValidRequest_TaskCreatedEventSaved()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "Read a book",
                DueDate = TestHelper.ToUtcTimestamp("2022-04-12"),
                Note = "Domain Driven Design",
            };

            var response = await client.CreateAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Single(events);
            AssertEquality.OfCreatedEvent(events[0], request, response);
        }
    }
}