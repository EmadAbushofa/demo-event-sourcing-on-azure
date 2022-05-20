using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksService.Create
{
    public class CreateTaskDuplicationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskDuplicationTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryEventStore();
                services.MockDuplicateTitleDetection(duplicateTitle: "My old task");
            });
        }


        [Fact]
        public async void Create_SendWithDuplicateTaskTitle_ThrowsAlreadyExistsException()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "My old task",
                DueDate = ProtoConverters.ToUtcTimestamp("2022-03-05"),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.CreateAsync(request));

            Assert.Equal(StatusCode.AlreadyExists, exception.StatusCode);
        }


        [Fact]
        public async void Create_SendUniqueTaskTitle_TaskCreatedEventSaved()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = Guid.NewGuid().ToString(),
                Title = "My new task",
                DueDate = ProtoConverters.ToUtcTimestamp("2022-03-05"),
            };

            var response = await client.CreateAsync(request);

            Assert.NotNull(response.Id);
        }
    }
}