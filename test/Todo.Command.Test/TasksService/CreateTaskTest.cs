using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Command.TodoProto;
using Xunit.Abstractions;

namespace Todo.Command.Test.TasksService
{
    public class CreateTaskTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper);
        }


        [Theory]
        [InlineData("Workout", "2022-03-27 14:22:09", "Take your proteins.")]
        public async void Create_SendValidRequest_TaskCreatedEventSaved(
            string title,
            string dueDateString,
            string note
        )
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var response = await client.CreateAsync(new CreateRequest()
            {
                UserId = Guid.NewGuid().ToString(),
                Title = title,
                DueDate = DateTime.Parse(dueDateString).ToUniversalTime().ToTimestamp(),
                Note = note,
            });

            Assert.NotNull(response.Id);
        }
    }
}