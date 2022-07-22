using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Todo.Query.Entities;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.InfoUpdated;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;
using AssertEquality = Todo.Query.Test.Helpers.AssertEquality;

namespace Todo.Query.Test.Live.HandlersTests
{
    public class TaskInfoUpdatedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;

        public TaskInfoUpdatedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
            _dbContextHelper = new DbContextHelper(_factory.Services);
        }


        [Fact]
        public async Task When_NewTaskInfoUpdatedEventArrived_TaskSaved()
        {
            var todoTask = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTask).Generate();

            await CommandServiceHelper.SendAsync(@event);
            await Task.Delay(5000);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            AssertEquality.OfEventAndTask(@event, todoTask);
        }

        [Fact]
        public async Task When_TaskInfoUpdatedEventConsumed_ReturnsNotification()
        {
            using var streamHelper = new NotificationsStreamHelper(_factory);

            var todoTaskBefore = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTaskBefore)
                .Generate();

            await CommandServiceHelper.SendAsync(@event);
            await Task.Delay(5000);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.Single(streamHelper.Notifications);
            AssertEquality.OfEventAndEntityAndNotification(@event, todoTaskAfter, streamHelper.Notifications[0]);
        }

        [Fact]
        public async Task When_NewTaskWithDuplicateTitleArrived_TaskSavedWithDifferentTitle()
        {
            var todoTask1 = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask(false));
            var todoTask2 = await _dbContextHelper.InsertAsync(TodoTaskFaker.WithSameUser(todoTask1, isCompleted: false));

            static Task SendAsync(TodoTask todoTask)
            {
                var @event = new TaskInfoUpdatedFaker()
                    .For(todoTask)
                    .RuleForTitle("My title")
                    .Generate();

                return CommandServiceHelper.SendAsync(@event).ResponseAsync;
            }

            await Task.WhenAll(SendAsync(todoTask1), SendAsync(todoTask2));
            await Task.Delay(5000);

            var todoTasks = await _dbContextHelper.Query(c => c.Tasks
                .Where(t => t.Id == todoTask1.Id || t.Id == todoTask2.Id)
                .ToListAsync());

            Assert.Contains(todoTasks, t => t.NormalizedTitle == "My title".ToUpper());
            Assert.Contains(todoTasks, t => t.NormalizedTitle.StartsWith("My title".ToUpper() + "_COPY"));
        }
    }
}
