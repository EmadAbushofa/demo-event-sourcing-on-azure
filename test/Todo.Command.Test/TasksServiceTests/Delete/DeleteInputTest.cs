using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions.Persistence;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.Created;
using Todo.Command.Test.Fakers.Deleted;
using Todo.Command.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksServiceTests.Delete
{
    public class DeleteInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly EventStoreHelper _eventStoreHelper;

        public DeleteInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryEventStore();
            });
            _eventStoreHelper = new EventStoreHelper(_factory.Services);
        }

        [Fact]
        public async Task Delete_DeleteExistingTask_TaskDeletedEventSaved()
        {
            var createdEvent = await _eventStoreHelper.GenerateAndAppendToStreamAsync(new TaskCreatedFaker());

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new DeleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var response = await client.DeleteAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(2, events.Count);
            AssertEquality.OfDeletedEvent(events[1], request, response, expectedSequence: 2);
        }

        [Theory]
        [InlineData(false, true, nameof(DeleteRequest.Id))]
        [InlineData(true, false, nameof(DeleteRequest.UserId))]
        public async Task Delete_SendInvalidRequest_ThrowsInvalidArgumentRpcException(
            bool validAggregateId,
            bool validUserId,
            string errorPropertyName
        )
        {
            var createdEvent = await _eventStoreHelper.GenerateAndAppendToStreamAsync(new TaskCreatedFaker());

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var id = validAggregateId ? createdEvent.AggregateId.ToString() : "wejoghfio";
            var userId = validUserId ? Guid.NewGuid().ToString() : " ";

            var request = new DeleteRequest()
            {
                Id = id,
                UserId = userId,
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.DeleteAsync(request));

            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
            Assert.Contains(
                exception.GetValidationErrors(),
                e => e.PropertyName.EndsWith(errorPropertyName)
            );
        }

        [Fact]
        public async Task Delete_SendInvalidId_ThrowsNotFoundRpcException()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new DeleteRequest()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.DeleteAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteOtherUsersTask_ThrowsNotFoundRpcException()
        {
            var createdEvent = await _eventStoreHelper.GenerateAndAppendToStreamAsync(new TaskCreatedFaker());

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new DeleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = Guid.NewGuid().ToString(),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.DeleteAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task Delete_DeleteAlreadyDeletedTask_ThrowsNotFoundRpcException()
        {
            var createdEvent = await _eventStoreHelper.GenerateAndAppendToStreamAsync(new TaskCreatedFaker());
            await _eventStoreHelper.GenerateAndAppendToStreamAsync(new TaskDeletedFaker().For(createdEvent));

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new DeleteRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.DeleteAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }
    }
}