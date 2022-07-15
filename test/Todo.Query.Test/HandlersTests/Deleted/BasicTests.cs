using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.Deleted;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests.Deleted
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
        public async Task When_NewTaskDeletedEventHandled_TaskDeleted()
        {
            var todoTask = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskDeletedFaker()
                .For(todoTask).Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.True(isHandled);
            Assert.Null(todoTask);
        }

        [Fact]
        public async Task When_DuplicateTaskDeletedEventHandled_DuplicateEventNotHandled()
        {
            var todoTask = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskDeletedFaker()
                .For(todoTask).Generate();

            await _eventHandlerHelper.HandleAsync(@event);

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.False(isHandled);
            Assert.Null(todoTask);
        }

        [Fact]
        public async Task When_TaskDeletedEventHandledBeforeCreatedEventArrives_EventNotHandled()
        {
            var @event = new TaskDeletedFaker()
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTask = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.False(isHandled);
            Assert.Null(todoTask);
        }

        [Fact]
        public async Task When_TaskDeletedEventWithOldSequenceHandled_EventIgnored()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskDeletedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence - 1)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.True(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }

        [Fact]
        public async Task When_TaskDeletedEventHandledPrematurely_EventNotHandled()
        {
            var todoTaskBefore = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskDeletedFaker()
                .For(todoTaskBefore, sequence: todoTaskBefore.Sequence + 2)
                .Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTaskAfter = await _dbContextHelper.Query(c => c.Tasks.SingleOrDefaultAsync());

            Assert.False(isHandled);
            AssertEquality.OfBoth(todoTaskBefore, todoTaskAfter);
        }
    }
}
