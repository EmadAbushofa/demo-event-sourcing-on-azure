using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.Uncompleted;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;
using AssertEquality = Todo.Query.Test.Helpers.AssertEquality;

namespace Todo.Query.Test.Live.HandlersTests
{
    public class TaskUncompletedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;

        public TaskUncompletedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
            _dbContextHelper = new DbContextHelper(_factory.Services);
        }

        [Fact]
        public async Task When_NewTaskUncompletedEventArrived_TaskSaved()
        {
            var todoTask = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask());

            var @event = new TaskUncompletedFaker()
                .For(todoTask).Generate();

            await CommandServiceHelper.SendAsync(@event);
            await Task.Delay(5000);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            AssertEquality.OfEventAndTask(@event, todoTask);
        }

        [Fact]
        public async Task When_TaskUncompletedEventConsumed_ReturnsNotification()
        {
            using var streamHelper = new NotificationsStreamHelper(_factory);

            var todoTaskBefore = await _dbContextHelper
                .InsertAsync(new TodoTaskFaker().GeneratWithRandomUniqueTitle(isCompleted: true));

            var @event = new TaskUncompletedFaker()
                .For(todoTaskBefore)
                .Generate();

            await CommandServiceHelper.SendAsync(@event);
            await Task.Delay(5000);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.Single(streamHelper.Notifications);
            AssertEquality.OfEventAndEntityAndNotification(@event, todoTaskAfter, streamHelper.Notifications[0]);
        }
    }
}
