using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Todo.Query.Extensions;
using Todo.Query.Test.Fakers.Created;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Client.DemoEventsProto;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests
{
    public class TaskCreatedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;

        public TaskCreatedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
            _dbContextHelper = new DbContextHelper(_factory.Services);
        }


        [Fact]
        public async Task When_NewTaskCreatedEventArrived_TaskSaved()
        {
            var @event = new TaskCreatedFaker().Generate();

            var client = CommandServiceHelper.CreateDemoEventsClient();

            await client.CreateAsync(new CreateRequest()
            {
                Id = @event.AggregateId.ToString(),
                DueDate = @event.Data.DueDate.ToUtcTimestamp(),
                Note = @event.Data.Note,
                Title = @event.Data.Title,
                UserId = @event.UserId,
            });

            await Task.Delay(5000);

            var todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            AssertEquality.OfEventAndTask(@event, todoTask);
        }


        [Fact]
        public async Task When_NewTaskWithDuplicateTitleArrived_TaskSavedWithDifferentNormalizedTitle()
        {
            var title = "My title " + Guid.NewGuid();

            var ids = await Generate2EventsWithSameTitle(title);

            await Task.Delay(5000);

            var todoTasks = await _dbContextHelper.Query(c => c.Tasks
                .Where(t => ids.Contains(t.Id))
                .ToListAsync());

            Assert.Contains(todoTasks, t => t.NormalizedTitle == title.ToUpper());
            Assert.Contains(todoTasks, t => t.NormalizedTitle.StartsWith(title.ToUpper() + "_COPY"));
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
