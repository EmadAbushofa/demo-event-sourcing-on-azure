using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.Uncompleted;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests.Uncompleted
{
    public class BasicTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;
        private readonly EventHandlerHelper _eventHandlerHelper;

        public BasicTests(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryDatabase();
            });

            _dbContextHelper = new DbContextHelper(_factory.Services);
            _eventHandlerHelper = new EventHandlerHelper(_factory.Services);
        }

        [Fact]
        public async Task When_NewTaskUncompletedEventHandled_TaskSaved()
        {
            var todoTask = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask());

            var @event = new TaskUncompletedFaker()
                .For(todoTask).Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask);
        }

        [Fact]
        public async Task When_DuplicateTaskUncompletedEventHandled_DuplicateEventIgnored()
        {
            var todoTask = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask());

            var @event = new TaskUncompletedFaker()
                .For(todoTask).Generate();

            await _eventHandlerHelper.HandleAsync(@event);

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask);
        }

        [Fact]
        public async Task When_TaskUncompletedEventHandledBeforeCreatedEventArrives_EventNotHandled()
        {
            var @event = new TaskUncompletedFaker()
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTask = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.False(isHandled);
            Assert.Null(todoTask);
        }

        [Fact]
        public async Task When_TaskUncompletedEventWithOldSequenceHandled_EventIgnored()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask());

            var @event = new TaskUncompletedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence - 1)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.True(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }

        [Fact]
        public async Task When_TaskUncompletedEventWithSameSequenceHandled_EventIgnored()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask());

            var @event = new TaskUncompletedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.True(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }

        [Fact]
        public async Task When_TaskUncompletedEventHandledPrematurely_EventNotHandled()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(TodoTaskFaker.GenerateCompletedTask());

            var @event = new TaskUncompletedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence + 2)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.False(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }
    }
}
