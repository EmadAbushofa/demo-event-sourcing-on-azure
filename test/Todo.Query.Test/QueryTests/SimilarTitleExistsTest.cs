using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Entities;
using Todo.Query.Infrastructure.Data;
using Todo.Query.Test.Client.TodoProto;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.HandlersTests
{
    public class SimilarTitleExistsTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public SimilarTitleExistsTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryDatabase();
            });
        }

        [Fact]
        public async Task SimilarTitleExists_QueryWithSimilarTitleAndUser_ReturnExists()
        {
            var todoTask = await GenerateTodoTaskAsync(isCompleted: false);

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new SimilarTitleExistsRequest()
            {
                UserId = todoTask.UserId,
                Title = " " + todoTask.Title.ToUpper() + " "
            };

            var response = await client.SimilarTitleExistsAsync(request);

            Assert.True(response.Exists);
            Assert.Equal(todoTask.Id.ToString(), response.Id);
        }

        [Fact]
        public async Task SimilarTitleExists_QueryCompletedTaskWithSimilarTitleAndUser_ReturnNotExists()
        {
            var todoTask = await GenerateTodoTaskAsync(isCompleted: true);

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new SimilarTitleExistsRequest()
            {
                UserId = todoTask.UserId,
                Title = todoTask.Title
            };

            var response = await client.SimilarTitleExistsAsync(request);

            Assert.False(response.Exists);
            Assert.Null(response.Id);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task SimilarTitleExists_QueryWithSimilarTitleOrSimilarUser_ReturnNotExists(
            bool sameUserId,
            bool sameTitle
        )
        {
            var todoTask = await GenerateTodoTaskAsync(isCompleted: false);

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new SimilarTitleExistsRequest()
            {
                UserId = sameUserId ? todoTask.UserId : Guid.NewGuid().ToString(),
                Title = sameTitle ? todoTask.Title : "Any"
            };

            var response = await client.SimilarTitleExistsAsync(request);

            Assert.False(response.Exists);
            Assert.Null(response.Id);
        }

        [Theory]
        [InlineData("some user id", " ")]
        [InlineData(" ", "Some title")]
        public async Task SimilarTitleExists_SendInvalidRequest_ReturnInvalidArgument(
            string? userId,
            string? title
        )
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new SimilarTitleExistsRequest()
            {
                UserId = userId,
                Title = title
            };

            var exception = await Assert.ThrowsAsync<RpcException>(() => client.SimilarTitleExistsAsync(request).ResponseAsync);

            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        }

        private async Task<TodoTask> GenerateTodoTaskAsync(bool isCompleted)
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

            var todoTask = new TodoTaskFaker()
                .RuleFor(e => e.IsCompleted, isCompleted)
                .Generate();

            await context.Tasks.AddAsync(todoTask);

            await context.SaveChangesAsync();

            return todoTask;
        }
    }
}
