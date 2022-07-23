using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.Deleted;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.Live.HandlersTests
{
    public class TaskDeletedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;

        public TaskDeletedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
            _dbContextHelper = new DbContextHelper(_factory.Services);
        }


        [Fact]
        public async Task When_NewTaskDeletedEventArrived_TaskSaved()
        {
            var todoTask = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskDeletedFaker()
                .For(todoTask).Generate();

            await CommandServiceHelper.SendAsync(@event);
            await Task.Delay(5000);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.Null(todoTask);
        }

        [Fact]
        public async Task When_TaskDeletedEventConsumed_ReturnsNotification()
        {
            using var streamHelper = new NotificationsStreamHelper(_factory);

            var todoTaskBefore = await _dbContextHelper.InsertAsync(new TodoTaskFaker().GeneratWithRandomUniqueTitle());

            var @event = new TaskDeletedFaker()
                .For(todoTaskBefore)
                .Generate();

            await CommandServiceHelper.SendAsync(@event);
            await Task.Delay(5000);

            Assert.Single(streamHelper.Notifications);
            AssertEquality.OfEventAndEntityAndNotification(@event, todoTaskBefore, streamHelper.Notifications[0]);
        }
    }
}
