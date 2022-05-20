using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstraction;
using Todo.Command.Test.Helpers;
using Todo.Command.TodoProto;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksService
{
    public class CreateTaskTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryEventStore();
            });
        }


        [Theory]
        [InlineData("1", "Workout", "2022-03-27", "Take your proteins.")]
        [InlineData("emad-bushofa", "Read a book", "2022-04-12", " ")]
        [InlineData("f52878b5-2908-4182-b933-c74ada709c7d", "Signup for a course", "2022-04-12", null)]
        public async void Create_SendValidRequest_TaskCreatedEventSaved(
            string userId,
            string title,
            string dueDateString,
            string note
        )
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = userId,
                Title = title,
                DueDate = TestHelper.ToUtcTimestamp(dueDateString),
                Note = note,
            };

            var response = await client.CreateAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Single(events);
            AssertEquality.OfCreatedEvent(events[0], request, response);
        }
    }
}