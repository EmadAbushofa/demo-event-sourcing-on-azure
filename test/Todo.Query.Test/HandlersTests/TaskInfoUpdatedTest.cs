using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.InfoUpdated;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests
{
    public class TaskInfoUpdatedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;
        private readonly EventHandlerHelper _eventHandlerHelper;

        public TaskInfoUpdatedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryDatabase();
            });

            _dbContextHelper = new DbContextHelper(_factory.Services);
            _eventHandlerHelper = new EventHandlerHelper(_factory.Services);
        }

        [Fact]
        public async Task When_NewTaskInfoUpdatedEventHandled_TaskSaved()
        {
            var todoTask = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTask).Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask);
        }

        [Fact]
        public async Task When_DuplicateTaskInfoUpdatedEventHandled_DuplicateEventIgnored()
        {
            var todoTask = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTask).Generate();

            await _eventHandlerHelper.HandleAsync(@event);

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask);
        }

        [Fact]
        public async Task When_TaskInfoUpdatedEventHandledBeforeCreatedEventArrives_EventNotHandled()
        {
            var @event = new TaskInfoUpdatedFaker()
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTask = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.False(isHandled);
            Assert.Null(todoTask);
        }

        [Fact]
        public async Task When_TaskInfoUpdatedEventWithOldSequenceHandled_EventIgnored()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence - 1)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.True(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }

        [Fact]
        public async Task When_TaskInfoUpdatedEventWithSameSequenceHandled_EventIgnored()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.True(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }

        [Fact]
        public async Task When_TaskInfoUpdatedEventHandledPrematurely_EventNotHandled()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskInfoUpdatedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence + 2)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.False(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }
    }
}
