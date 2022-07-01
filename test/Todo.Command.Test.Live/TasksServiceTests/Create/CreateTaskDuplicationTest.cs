using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions;
using Todo.Command.Events;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.TaskCreated;
using Todo.Command.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.Live.TasksServiceTests.Create
{
    public class CreateTaskDuplicationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskDuplicationTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
            });
        }

        [Fact]
        public async void Create_SameUserCreateTaskWithDuplicateTitle_ReturnAlreadyExists()
        {
            var savedEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = savedEvent.UserId,
                Title = savedEvent.Data.Title,
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
                Note = "Domain Driven Design",
            };

            await Task.Delay(5000);

            var exception = await Assert.ThrowsAsync<RpcException>(() => client.CreateAsync(request).ResponseAsync);

            Assert.Equal(StatusCode.AlreadyExists, exception.StatusCode);
        }

        [Fact]
        public async void Create_SameUserCreateTaskWithDuplicateTitleOfACompletedTask_TaskCreatedEventSaved()
        {
            var savedEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = savedEvent.UserId,
                Title = savedEvent.Data.Title,
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
                Note = "Domain Driven Design",
            };

            await client.CreateAsync(request);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async void Create_SendNewTitleWithSameUserOrDuplicateTitleFromOtherUser_TaskCreatedEventSaved(
            bool sameUserId,
            bool sameTitle
        )
        {
            var savedEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = sameUserId ? savedEvent.UserId : Guid.NewGuid().ToString(),
                Title = sameTitle ? savedEvent.Data.Title : Guid.NewGuid().ToString(),
                DueDate = ProtoConverters.ToUtcTimestamp("2022-04-12"),
                Note = "Domain Driven Design",
            };

            await client.CreateAsync(request);
        }


        private async Task<TaskCreatedEvent> GenerateAndAppendToStreamAsync()
        {
            var eventStore = _factory.Services.GetRequiredService<IEventStore>();

            var taskCreatedEvent = new TaskCreatedEventFaker()
                .Generate();

            await eventStore.AppendToStreamAsync(taskCreatedEvent);

            return taskCreatedEvent;
        }
    }
}