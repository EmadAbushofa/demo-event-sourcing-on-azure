using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions;
using Todo.Command.Events;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.Completed;
using Todo.Command.Test.Fakers.Created;
using Todo.Command.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksServiceTests.Complete
{
    public class CompleteInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CompleteInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryEventStore();
            });
        }

        [Fact]
        public async Task Complete_SendValidRequest_TaskCompletedEventSaved()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync(new TaskCreatedFaker());

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CompleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var response = await client.CompleteAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(2, events.Count);
            AssertEquality.OfCompletedEvent(events[1], request, response, expectedSequence: 2);
        }

        [Theory]
        [InlineData(false, true, nameof(CompleteRequest.Id))]
        [InlineData(true, false, nameof(CompleteRequest.UserId))]
        public async Task Complete_SendInvalidRequest_ThrowsInvalidArgumentRpcException(
            bool validAggregateId,
            bool validUserId,
            string errorPropertyName
        )
        {
            var createdEvent = await GenerateAndAppendToStreamAsync(new TaskCreatedFaker());

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var id = validAggregateId ? createdEvent.AggregateId.ToString() : "wejoghfio";
            var userId = validUserId ? Guid.NewGuid().ToString() : " ";

            var request = new CompleteRequest()
            {
                Id = id,
                UserId = userId,
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.CompleteAsync(request));

            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
            Assert.Contains(
                exception.GetValidationErrors(),
                e => e.PropertyName.EndsWith(errorPropertyName)
            );
        }

        [Fact]
        public async Task Complete_SendInvalidId_ThrowsNotFoundRpcException()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CompleteRequest()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.CompleteAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task Complete_CompleteOtherUsersTask_ThrowsNotFoundRpcException()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync(new TaskCreatedFaker());

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CompleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.CompleteAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task Complete_CompleteAlreadyCompletedTask_ThrowsFailedPreconditionRpcException()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync(new TaskCreatedFaker());
            await GenerateAndAppendToStreamAsync(new TaskCompletedFaker().For(createdEvent));

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CompleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.CompleteAsync(request));

            Assert.Equal(StatusCode.FailedPrecondition, exception.StatusCode);
        }

        private async Task<TaskCreated> GenerateAndAppendToStreamAsync(TaskCreatedFaker faker)
        {
            var eventStore = _factory.Services.GetRequiredService<IEventStore>();

            var taskCreated = faker.Generate();

            await eventStore.AppendToStreamAsync(taskCreated);

            return taskCreated;
        }

        private async Task<TaskCompleted> GenerateAndAppendToStreamAsync(TaskCompletedFaker faker)
        {
            var eventStore = _factory.Services.GetRequiredService<IEventStore>();

            var taskCompleted = faker.Generate();

            await eventStore.AppendToStreamAsync(taskCompleted);

            return taskCompleted;
        }
    }
}