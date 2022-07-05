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
                services.DisableQueryDuplicateDetection();
            });
        }


        [Theory]
        [InlineData("Workout", "Take your proteins.")]
        [InlineData("Read a book", " ")]
        [InlineData("Signup for a course", null)]
        public async Task UpdateInfo_SendValidRequest_TaskUpdateInfodEventSaved(
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


        //[Theory]
        //[InlineData(false, "Workout", "2022-03-27", nameof(UpdateInfoRequest.UserId))]
        //[InlineData(true, " ", "2022-03-27", nameof(UpdateInfoRequest.Title))]
        //[InlineData(true, "Read a book", "1800-03-27", nameof(UpdateInfoRequest.DueDate))]
        //[InlineData(true, "Read a book", "2200-03-27", nameof(UpdateInfoRequest.DueDate))]
        //public async Task UpdateInfo_SendInvalidRequest_ThrowsInvalidArgumentRpcException(
        //    bool validUserId,
        //    string title,
        //    string dueDateString,
        //    string errorPropertyName
        //)
        //{
        //    var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

        //    var userId = validUserId ? Guid.NewGuid().ToString() : " ";

        //    var request = new UpdateInfoRequest()
        //    {
        //        UserId = userId,
        //        Title = title,
        //        DueDate = ProtoConverters.ToUtcTimestamp(dueDateString),
        //    };

        //    var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.UpdateInfoAsync(request));

        //    Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
        //    Assert.Contains(
        //        exception.GetValidationErrors(),
        //        e => e.PropertyName.EndsWith(errorPropertyName)
        //    );
        //}

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