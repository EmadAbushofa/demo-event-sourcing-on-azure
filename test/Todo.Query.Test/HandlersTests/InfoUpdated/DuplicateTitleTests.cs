using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.InfoUpdated;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests.InfoUpdated
{
    public class DuplicateTitleTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;
        private readonly EventHandlerHelper _eventHandlerHelper;

        public DuplicateTitleTests(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryDatabase();
            });

            _dbContextHelper = new DbContextHelper(_factory.Services);
            _eventHandlerHelper = new EventHandlerHelper(_factory.Services);
        }

        [Fact]
        public async Task When_NewTaskInfoUpdatedEventHandledWithSameTitleOfAnotherTask_TaskSavedWithDifferentTitle()
        {
            var todoTask1 = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask(false));
            var todoTask2 = await _dbContextHelper.InsertAsync(TodoTaskFaker.WithSameUser(todoTask1, isCompleted: false));

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTask2)
                .RuleForTitle(todoTask1.Title)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask2 = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask2, isUnique: false);
        }

        [Fact]
        public async Task When_NewTaskInfoUpdatedEventHandledWithSameTitleOfAnotherUsersTask_TaskSavedCorrectly()
        {
            var todoTask1 = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask(false));
            var todoTask2 = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask(false));

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTask2)
                .RuleForTitle(todoTask1.Title)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask2 = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask2, isUnique: true);
        }

        [Theory]
        [InlineData(false, true)]
        [InlineData(true, false)]
        public async Task When_NewTaskInfoUpdatedEventHandledWithSameTitleOfAnotherCompletedTask_TaskSavedWithDifferentTitle(
            bool task1Completed,
            bool task2Completed
        )
        {
            var todoTask1 = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask(task1Completed));
            var todoTask2 = await _dbContextHelper.InsertAsync(TodoTaskFaker.WithSameUser(todoTask1, task2Completed));

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTask2)
                .RuleForTitle(todoTask1.Title)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask2 = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask2, isUnique: true);
        }
    }
}
