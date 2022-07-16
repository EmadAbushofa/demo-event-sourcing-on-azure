using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.Uncompleted;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Client.DemoEventsProto;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;
using AssertEquality = Todo.Query.Test.Helpers.AssertEquality;

namespace Todo.Query.Test.HandlersTests
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

            var client = CommandServiceHelper.CreateDemoEventsClient();

            await client.UncompleteAsync(new CompleteRequest()
            {
                Id = @event.AggregateId.ToString(),
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });

            await Task.Delay(5000);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            AssertEquality.OfEventAndTask(@event, todoTask);
        }
    }
}
