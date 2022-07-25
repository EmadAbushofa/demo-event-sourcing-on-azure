using Microsoft.AspNetCore.Mvc.Testing;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Todo.ApiGateway.Test.Live.TodoProto.Channel;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.ChannelTests
{
    public class CompleteTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CompleteTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task Complete_SendValidCommand_NotificationReturned()
        {
            var streamHelper = new NotificationsStreamHelper(_factory, "Emad");

            var id = await CreateAsync("Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.PatchJsonAsync<InputResponse>($"api/todo-tasks/{id}/complete");

            await Task.Delay(8000);

            Assert.Equal(2, streamHelper.Notifications.Count);
            Assert.NotEmpty(streamHelper.Notifications[1].Message);
            Assert.Equal(response.Id, streamHelper.Notifications[1].TodoTaskId);
            Assert.Equal(NotificationStatus.Succeed, streamHelper.Notifications[1].Status);
        }

        [Fact]
        public async Task Complete_SendValidCommandFromOtherUser_NoNotificationReturned()
        {
            var streamHelper = new NotificationsStreamHelper(_factory, "Emad");

            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClientWithUser("Yhwach");

            await client.PatchJsonAsync<InputResponse>($"api/todo-tasks/{id}/complete");

            await Task.Delay(8000);

            Assert.Empty(streamHelper.Notifications);
        }


        private async Task<string> CreateAsync(string user)
        {
            var client = _factory.CreateClientWithUser(user);

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = "any title"
            };

            var response = await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);

            return response?.Id ?? throw new InvalidOperationException();
        }
    }
}
