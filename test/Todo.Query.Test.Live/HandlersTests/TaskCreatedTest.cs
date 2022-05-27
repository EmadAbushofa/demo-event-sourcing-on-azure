using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Infrastructure.Data;
using Todo.Query.Test.Fakers.TaskCreated;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Client.DemoEventsProto;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests
{
    public class TaskCreatedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public TaskCreatedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
        }


        [Fact]
        public async void When_NewTaskCreatedEventArrived_TaskSaved()
        {
            var @event = new TaskCreatedEventFaker().Generate();

            var client = CommandServiceHelper.CreateDemoEventsClient();

            await client.CreateAsync(new CreateRequest()
            {
                Id = @event.AggregateId.ToString(),
                DueDate = @event.Data.DueDate.ToUtcTimestamp(),
                Note = @event.Data.Note,
                Title = @event.Data.Title,
                UserId = @event.UserId,
            });

            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

            var todoTask = await context.Tasks.FindAsync(@event.AggregateId);

            AssertEquality.OfEventAndTask(@event, todoTask);
        }
    }
}
