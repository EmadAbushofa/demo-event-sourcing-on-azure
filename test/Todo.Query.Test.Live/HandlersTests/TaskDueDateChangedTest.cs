using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Extensions;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Fakers.DueDateChanged;
using Todo.Query.Test.Helpers;
using Todo.Query.Test.Live.Client.DemoEventsProto;
using Todo.Query.Test.Live.Helpers;
using Xunit.Abstractions;
using AssertEquality = Todo.Query.Test.Helpers.AssertEquality;

namespace Todo.Query.Test.HandlersTests
{
    public class TaskDueDateChangedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;

        public TaskDueDateChangedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
            _dbContextHelper = new DbContextHelper(_factory.Services);
        }


        [Fact]
        public async Task When_NewTaskDueDateChangedEventArrived_TaskSaved()
        {
            var todoTask = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate());

            var @event = new TaskDueDateChangedFaker()
                .For(todoTask).Generate();

            var client = CommandServiceHelper.CreateDemoEventsClient();

            await client.ChangeDueDateAsync(new ChangeDueDateRequest()
            {
                Id = @event.AggregateId.ToString(),
                DueDate = @event.Data.DueDate.ToUtcTimestamp(),
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });

            await Task.Delay(5000);

            todoTask = await _dbContextHelper.Query(c => c.Tasks.FindAsync(@event.AggregateId));

            AssertEquality.OfEventAndTask(@event, todoTask);
        }
    }
}
