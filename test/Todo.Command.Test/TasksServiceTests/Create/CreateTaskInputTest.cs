using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions;
using Todo.Command.Test.Client.TodoProto;
using Todo.Command.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksServiceTests.Create
{
    public class CreateTaskInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryEventStore();
                services.DisableQueryDuplicateDetection();
            });
        }


        [Theory]
        [InlineData("1", "Workout", "2022-03-27", "Take your proteins.")]
        [InlineData("emad-bushofa", "Read a book", "2022-04-12", " ")]
        [InlineData("f52878b5-2908-4182-b933-c74ada709c7d", "Signup for a course", "2022-04-12", null)]
        public async void Create_SendValidRequest_TaskCreatedEventSaved(
            string userId,
            string title,
            string dueDateString,
            string note
        )
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var request = new CreateRequest()
            {
                UserId = userId,
                Title = title,
                DueDate = ProtoConverters.ToUtcTimestamp(dueDateString),
                Note = note,
            };

            var response = await client.CreateAsync(request);

            var stream = _factory.Services.GetRequiredService<IEventStore>();

            var @events = await stream.GetStreamAsync(response.Id);

            Assert.Single(events);
            AssertEquality.OfCreatedEvent(events[0], request, response);
        }


        [Theory]
        [InlineData(false, "Workout", "2022-03-27", nameof(CreateRequest.UserId))]
        [InlineData(true, " ", "2022-03-27", nameof(CreateRequest.Title))]
        [InlineData(true, "Read a book", "1800-03-27", nameof(CreateRequest.DueDate))]
        [InlineData(true, "Read a book", "2200-03-27", nameof(CreateRequest.DueDate))]
        public async void Create_SendInvalidRequest_ThrowsInvalidArgumentRpcException(
            bool validUserId,
            string title,
            string dueDateString,
            string errorPropertyName
        )
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var userId = validUserId ? Guid.NewGuid().ToString() : " ";

            var request = new CreateRequest()
            {
                UserId = userId,
                Title = title,
                DueDate = ProtoConverters.ToUtcTimestamp(dueDateString),
            };

            var exception = await Assert.ThrowsAsync<RpcException>(async () => await client.CreateAsync(request));

            Assert.Equal(StatusCode.InvalidArgument, exception.StatusCode);
            Assert.Contains(
                exception.GetValidationErrors(),
                e => e.PropertyName.EndsWith(errorPropertyName)
            );
        }
    }
}