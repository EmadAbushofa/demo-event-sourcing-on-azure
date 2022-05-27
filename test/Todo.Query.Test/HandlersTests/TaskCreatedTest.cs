using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Abstractions;
using Todo.Query.Features.Create;
using Todo.Query.Infrastructure.Data;
using Todo.Query.Test.Fakers.TaskCreated;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests
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
        public async void When_NewTaskCreatedEventHandled_TaskSaved()
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


        [Fact]
        public async void When_DuplicateTaskCreatedEventHandled_TaskSaved()
        {
            bool isHandled;

            using (var scope = _factory.Services.CreateScope())
            {
                var @event = new TaskCreatedEventFaker().Generate();

                await CreateTaskFromEventAsync(@event, scope);

                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                isHandled = await mediator.Send(@event);
            }

            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

                var tasks = await context.Tasks.ToListAsync();

                Assert.True(isHandled);
                Assert.Single(tasks);
            }
        }

        private static async Task CreateTaskFromEventAsync(TaskCreatedEvent @event, IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();
            await context.Tasks.AddAsync(TodoTask.FromCreatedEvent(@event));
            await context.SaveChangesAsync();
        }
    }
}
