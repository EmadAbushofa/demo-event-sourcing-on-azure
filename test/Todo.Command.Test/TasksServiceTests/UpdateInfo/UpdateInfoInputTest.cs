using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions;
using Todo.Command.Events;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Fakers.TaskCreated;
using Todo.Command.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksServiceTests.UpdateInfo
{
    public class UpdateInfoInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UpdateInfoInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryEventStore();
            });
        }


        [Theory]
        [InlineData("Workout", "Take your proteins.")]
        [InlineData("Read a book", " ")]
        [InlineData("Signup for a course", null)]
        public async Task UpdateInfo_SendValidRequest_TaskInfoUpdatedEventSaved(
            string title,
            string note
        )
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new UpdateInfoRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                Title = title,
                Note = note,
            };

            var response = await client.UpdateInfoAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Equal(2, events.Count);
            AssertEquality.OfInfoUpdatedEvent(events[1], request, response, expectedSequence: 2);
        }

        [Theory]
        [InlineData(false, true, "Workout", nameof(UpdateInfoRequest.Id))]
        [InlineData(true, false, "Workout", nameof(UpdateInfoRequest.UserId))]
        [InlineData(true, true, " ", nameof(UpdateInfoRequest.Title))]
        public async Task UpdateInfo_SendInvalidRequest_ThrowsInvalidArgumentRpcException(
            bool validAggregateId,
            bool validUserId,
            string title,
            string errorPropertyName
        )
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var id = validAggregateId ? createdEvent.AggregateId.ToString() : "wejoghfio";
            var userId = validUserId ? Guid.NewGuid().ToString() : " ";

            var request = new UpdateInfoRequest()
            {
                Id = id,
                UserId = userId,
                Title = title,
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.UpdateInfoAsync(request));

            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
            Assert.Contains(
                exception.GetValidationErrors(),
                e => e.PropertyName.EndsWith(errorPropertyName)
            );
        }

        [Fact]
        public async Task UpdateInfo_SendInvalidId_ThrowsNotFoundRpcException()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new UpdateInfoRequest()
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Title = "Some title",
                Note = "Note"
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.UpdateInfoAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task UpdateInfo_UpdateOtherUsersTask_ThrowsNotFoundRpcException()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new UpdateInfoRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = Guid.NewGuid().ToString(),
                Title = "Some title",
                Note = "Note"
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.UpdateInfoAsync(request));

            Assert.Equal(StatusCode.NotFound, exception.StatusCode);
        }

        [Fact]
        public async Task UpdateInfo_NothingChanged_NoNewEventSaved()
        {
            var createdEvent = await GenerateAndAppendToStreamAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new UpdateInfoRequest()
            {
                Id = createdEvent.AggregateId.ToString(),
                UserId = createdEvent.UserId,
                Title = createdEvent.Data.Title,
                Note = createdEvent.Data.Note,
            };

            var response = await client.UpdateInfoAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Single(events);
            Assert.Equal(EventType.TaskCreated, events[0].Type);
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