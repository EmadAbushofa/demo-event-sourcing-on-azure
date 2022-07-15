using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Query.Entities;
using Todo.Query.Infrastructure.Data;
using Todo.Query.Test.Client.TodoProto;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.QueryTests
{
    public class FindTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FindTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryDatabase();
            });
        }

        [Fact]
        public async Task Find_QueryExistingEntity_ReturnExpectedResponse()
        {
            var todoTask = await GenerateTodoTaskAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new FindRequest()
            {
                Id = todoTask.Id.ToString()
            };

            var response = await client.FindAsync(request);

            AssertEquality.OfTaskAndResponse(todoTask, response);
        }

        [Fact]
        public async Task Find_QueryNonExistingEntity_ReturnNotFound()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new FindRequest()
            {
                Id = Guid.NewGuid().ToString()
            };

            var exception = await Assert.ThrowsAsync<RpcException>(() => client.FindAsync(request).ResponseAsync);

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
            Assert.NotEmpty(exception.Status.Detail);
        }

        [Theory]
        [InlineData("")]
        [InlineData("not guid")]
        public async Task Find_QueryWithInvalidId_ReturnInvalidArgument(string id)
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new FindRequest()
            {
                Id = id
            };

            var exception = await Assert.ThrowsAsync<RpcException>(() => client.FindAsync(request).ResponseAsync);

            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
            Assert.NotEmpty(exception.Status.Detail);
        }


        private async Task<TodoTask> GenerateTodoTaskAsync()
        {
            using var scope = _factory.Services.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<TodoTasksDbContext>();

            var todoTask = new TodoTaskFaker().Generate();

            await context.Tasks.AddAsync(todoTask);

            await context.SaveChangesAsync();

            return todoTask;
        }
    }
}
