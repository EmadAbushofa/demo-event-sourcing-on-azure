using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.Created;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests.Created
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
        public async Task When_NewTaskWithDuplicateTitleArrived_TaskSavedWithDifferentTitle()
        {
            var (first, second) = new TaskCreatedFaker().Generate2EventsWithSameTitle();

            await Task.WhenAll(
                _eventHandlerHelper.HandleAsync(first),
                _eventHandlerHelper.HandleAsync(second)
            );

            var todoTasks = await _dbContextHelper.Query(c => c.Tasks.ToListAsync());

            Assert.Collection(
                todoTasks,
                t => AssertEquality.OfEventAndTask(first, t, isUnique: true),
                t => AssertEquality.OfEventAndTask(second, t, isUnique: false)
            );
        }

        [Fact]
        public async Task When_NewTasksWithDifferentTitleForSameUserArrived_TaskSavedCorrectly()
        {
            var (first, second) = new TaskCreatedFaker().Generate2EventsForSameUser();

            await Task.WhenAll(
                _eventHandlerHelper.HandleAsync(first),
                _eventHandlerHelper.HandleAsync(second)
            );

            var todoTasks = await _dbContextHelper.Query(c => c.Tasks.ToListAsync());

            Assert.Collection(
                todoTasks,
                t => AssertEquality.OfEventAndTask(first, t, isUnique: true),
                t => AssertEquality.OfEventAndTask(second, t, isUnique: true)
            );
        }

        [Fact]
        public async Task When_NewTaskWithDuplicateTitleForDifferentUserArrived_BothSavedCorrectly()
        {
            var (first, second) = new TaskCreatedFaker().Generate2EventsWithSameTitle(sameUser: false);

            await Task.WhenAll(
                _eventHandlerHelper.HandleAsync(first),
                _eventHandlerHelper.HandleAsync(second)
            );

            var todoTasks = await _dbContextHelper.Query(c => c.Tasks.ToListAsync());

            Assert.Collection(
                todoTasks,
                t => AssertEquality.OfEventAndTask(first, t, isUnique: true),
                t => AssertEquality.OfEventAndTask(second, t, isUnique: true)
            );
        }

        [Fact]
        public async Task When_NewTaskWithDuplicateTitleOfCompletedTaskArrived_NewTaskSaved()
        {
            var todoTask = await _dbContextHelper
                .InsertAsync(TodoTaskFaker.GenerateCompletedTask());

            var sameTitleAndUserTaskCreatedEvent = new TaskCreatedFaker()
                .RuleForTitle(todoTask.Title)
                .RuleFor(u => u.UserId, todoTask.UserId)
                .Generate();

            await _eventHandlerHelper.HandleAsync(sameTitleAndUserTaskCreatedEvent);

            var todoTasks = await _dbContextHelper.Query(c => c.Tasks.ToListAsync());

            Assert.All(todoTasks, t =>
            {
                Assert.Equal(todoTask.Title, t.Title);
                Assert.True(t.IsUniqueTitle);
            });
        }
    }
}
