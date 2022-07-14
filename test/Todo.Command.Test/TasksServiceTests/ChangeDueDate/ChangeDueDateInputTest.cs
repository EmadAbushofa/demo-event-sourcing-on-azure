using Calzolari.Grpc.Net.Client.Validation;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions;
using Todo.Command.Events;
using Todo.Command.Extensions;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.Created;
using Todo.Command.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksServiceTests.ChangeDueDate
{
    public class ChangeDueDateInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ChangeDueDateInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryEventStore();
            });
        }


        [Theory]
        [InlineData("2020-01-01")]
        [InlineData("2022-03-25")]
        [InlineData("2024-12-31")]
        public async Task ChangeDueDate_SendValidRequest_TaskDueDateChangedEventSaved(string dueDate)
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new ChangeDueDateRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                DueDate = ProtoConverters.ToUtcTimestamp(dueDate),
            };

            var response = await client.ChangeDueDateAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(2, events.Count);
            AssertEquality.OfDueDateChangedEvent(events[1], request, response, expectedSequence: 2);
        }

        [Theory]
        [InlineData(false, true, "2022-03-27", nameof(ChangeDueDateRequest.Id))]
        [InlineData(true, false, "2022-03-27", nameof(ChangeDueDateRequest.UserId))]
        [InlineData(true, true, "1800-03-27", nameof(ChangeDueDateRequest.DueDate))]
        [InlineData(true, true, "2200-03-27", nameof(ChangeDueDateRequest.DueDate))]
        public async Task ChangeDueDate_SendInvalidRequest_ThrowsInvalidArgumentRpcException(
            bool validAggregateId,
            bool validUserId,
            string dueDate,
            string errorPropertyName
        )
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var id = validAggregateId ? createdEvent.AggregateId.ToString() : "wejoghfio";
            var userId = validUserId ? Guid.NewGuid().ToString() : " ";

            var request = new ChangeDueDateRequest()
            {
                Id = id,
                UserId = userId,
                DueDate = ProtoConverters.ToUtcTimestamp(dueDate),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.ChangeDueDateAsync(request));

            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
            Assert.Contains(
                exception.GetValidationErrors(),
                e => e.PropertyName.EndsWith(errorPropertyName)
            );
        }

        [Fact]
        public async Task ChangeDueDate_SendInvalidId_ThrowsNotFoundRpcException()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new ChangeDueDateRequest()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                DueDate = DateTime.UtcNow.ToTimestamp(),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.ChangeDueDateAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task ChangeDueDate_ChangeOtherUsersTask_ThrowsNotFoundRpcException()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new ChangeDueDateRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = Guid.NewGuid().ToString(),
                DueDate = DateTime.UtcNow.ToTimestamp(),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.ChangeDueDateAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task ChangeDueDate_NothingChanged_NoNewEventSaved()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new ChangeDueDateRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                DueDate = createdEvent.Data.DueDate.ToTimestamp(),
            };

            var response = await client.ChangeDueDateAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Single(events);
            Assert.Equal(nameof(TaskCreated), events[0].Type);
        }

        private async Task<TaskCreated> GenerateAndAppendToStreamAsync()
        {
            var eventStore = _factory.Services.GetRequiredService<IEventStore>();

            var taskCreatedEvent = new TaskCreatedFaker()
                .Generate();

            await eventStore.AppendToStreamAsync(taskCreatedEvent);

            return taskCreatedEvent;
        }
    }
}