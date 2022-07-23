using Microsoft.AspNetCore.Mvc.Testing;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Todo.ApiGateway.Test.Live.TodoProto.Channel;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.ChannelTests
{
    public class CreateTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task Create_SendValidCommand_NotificationReturned()
        {
            var streamHelper = new NotificationsStreamHelper(_factory, "Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = $"My title {DateTime.UtcNow.Ticks}"
            };

            var response = await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);

            await Task.Delay(8000);

            Assert.Single(streamHelper.Notifications);
            Assert.NotEmpty(streamHelper.Notifications[0].Message);
            Assert.Equal(response.Id, streamHelper.Notifications[0].TodoTaskId);
            Assert.Equal(NotificationStatus.Succeed, streamHelper.Notifications[0].Status);
        }

        [Fact]
        public async Task Create_SendValidCommandFromOtherUser_NoNotificationReturned()
        {
            var streamHelper = new NotificationsStreamHelper(_factory, "Emad");

            var client = _factory.CreateClientWithUser("Yhwach");

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = $"My title {DateTime.UtcNow.Ticks}"
            };

            await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);

            await Task.Delay(8000);

            Assert.Empty(streamHelper.Notifications);
        }
    }
}
