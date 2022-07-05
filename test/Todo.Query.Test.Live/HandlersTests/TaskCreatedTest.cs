using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
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
        public async Task When_NewTaskCreatedEventArrived_TaskSaved()
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

            await Task.Delay(5000);

            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

            var todoTask = await context.Tasks.FindAsync(@event.AggregateId);

            AssertEquality.OfEventAndTask(@event, todoTask);
        }


        [Fact]
        public async Task When_NewTaskWithDuplicateTitleArrived_TaskSavedWithDifferentTitle()
        {
            var title = "My title " + Guid.NewGuid();

            var ids = await Generate2EventsWithSameTitle(title);

            using var scope = _factory.Services.CreateScope();

            await Task.Delay(5000);

            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

            var todoTasks = await context.Tasks
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            Assert.Contains(todoTasks, t => t.Title == title);
            Assert.Contains(todoTasks, t => t.Title.StartsWith(title + "_Copy"));
        }

        private static async Task<List<Guid>> Generate2EventsWithSameTitle(string title)
        {
            var client = CommandServiceHelper.CreateDemoEventsClient();

            var userId = Guid.NewGuid().ToString();

            var ids = new List<Guid>();

            Task CreateAsync()
            {
                var id = Guid.NewGuid();
                ids.Add(id);

                return client.CreateAsync(new CreateRequest()
                {
                    Id = id.ToString(),
                    DueDate = DateTime.UtcNow.ToUtcTimestamp(),
                    Title = title,
                    UserId = userId,
                }).ResponseAsync;
            }

            await Task.WhenAll(CreateAsync(), CreateAsync());

            return ids;
        }
    }
}
