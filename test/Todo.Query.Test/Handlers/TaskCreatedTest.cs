using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Features.Create;
using Todo.Query.Infrastructure.Data;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.Handlers
{
    public class TaskCreatedTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public TaskCreatedTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryDatabase();
            });
        }


        [Fact]
        public async void TaskCreated_HandleArrivedEvent_TaskSavedInDb()
        {
            bool isHandled;
            TaskCreatedEvent @event;

            using (var scope = _factory.Services.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                @event = new TaskCreatedEventFaker().Generate();

                isHandled = await mediator.Send(@event);
            }

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

                var todoTask = await context.Tasks.FindAsync(@event.AggregateId);

                Assert.True(isHandled);
                AssertEquality.OfEventAndTask(@event, todoTask);
            }
        }
    }
}
