using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Todo.Query.Test.Fakers.Created;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests.Created
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
        public async Task When_NewTaskCreatedEventHandled_TaskSaved()
        {
            var @event = new TaskCreatedFaker().Generate();

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.True(isHandled);
            AssertEquality.OfEventAndTask(@event, todoTask);
        }

        [Fact]
        public async Task When_DuplicateTaskCreatedEventHandled_DuplicateEventIgnored()
        {
            var @event = new TaskCreatedFaker().Generate();
            await _eventHandlerHelper.HandleAsync(@event);

            var isHandled = await _eventHandlerHelper.TryHandleAsync(@event);

            var tasks = await _dbContextHelper.Query(c => c.Tasks.ToListAsync());

            Assert.True(isHandled);
            Assert.Single(tasks);
        }
    }
}
