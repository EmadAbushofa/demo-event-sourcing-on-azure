using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Entities;
using Todo.Query.EventHandlers.Created;
using Todo.Query.Infrastructure.Data;
using Todo.Query.Test.Fakers.Created;
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
        public async Task When_NewTaskCreatedEventHandled_TaskSaved()
        {
            bool isHandled;
            TaskCreated @event;

            using (var scope = _factory.Services.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                @event = new TaskCreatedFaker().Generate();

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
        public async Task When_DuplicateTaskCreatedEventHandled_TaskSaved()
        {
            bool isHandled;

            using (var scope = _factory.Services.CreateScope())
            {
                var @event = new TaskCreatedFaker().Generate();

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

        [Fact]
        public async Task When_NewTaskWithDuplicateTitleArrived_TaskSavedWithDifferentTitle()
        {
            var title = "My title " + Guid.NewGuid();

            var ids = await Generate2EventsWithSameTitle(_factory.Services, title);

            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

            var todoTasks = await context.Tasks
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();

            Assert.Contains(todoTasks, t => t.Title == title);
            Assert.Contains(todoTasks, t => t.Title.StartsWith(title + "_Copy"));
        }

        private static async Task CreateTaskFromEventAsync(TaskCreated @event, IServiceScope scope)
        {
            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();
            await context.Tasks.AddAsync(TodoTask.FromCreatedEvent(@event));
            await context.SaveChangesAsync();
        }

        private static async Task<List<Guid>> Generate2EventsWithSameTitle(IServiceProvider provider, string title)
        {
            var userId = Guid.NewGuid().ToString();

            var ids = new List<Guid>();

            Task SendAsync()
            {
                var id = Guid.NewGuid();
                ids.Add(id);

                var scope = provider.CreateScope();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                var dataFaker = new TaskCreatedDataFaker()
                    .RuleFor(e => e.Title, title);

                var @event = new TaskCreatedFaker()
                    .RuleFor(e => e.AggregateId, id)
                    .RuleFor(e => e.UserId, userId)
                    .RuleFor(e => e.Data, dataFaker)
                    .Generate();

                return mediator.Send(@event);
            }

            await Task.WhenAll(SendAsync(), SendAsync());

            return ids;
        }
    }
}
