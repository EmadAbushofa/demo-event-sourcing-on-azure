using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.Deleted;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Client.DemoEventsProto;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests
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

            var client = CommandServiceHelper.CreateDemoEventsClient();

            await client.DeleteAsync(new DeleteRequest()
            {
                Id = @event.AggregateId.ToString(),
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });

            await Task.Delay(5000);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            Assert.Null(todoTask);
        }
    }
}
