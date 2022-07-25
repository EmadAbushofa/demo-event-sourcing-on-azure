using Microsoft.AspNetCore.Mvc.Testing;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Todo.ApiGateway.Test.Live.TodoProto.Channel;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.ChannelTests
{
    public class UpdateInfoTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UpdateInfoTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task UpdateInfo_SendValidCommand_NotificationReturned()
        {
            var streamHelper = new NotificationsStreamHelper(_factory, "Emad");
            var id = await CreateAsync("Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var input = new UpdateInfoTaskInput()
            {
                Note = "New note",
                Title = $"My new title {DateTime.UtcNow.Ticks}"
            };
            var response = await client.PutJsonAsync<InputResponse>($"api/todo-tasks/{id}/update-info", input);

            await Task.Delay(8000);

            Assert.Equal(2, streamHelper.Notifications.Count);
            Assert.NotEmpty(streamHelper.Notifications[1].Message);
            Assert.Equal(response.Id, streamHelper.Notifications[1].TodoTaskId);
            Assert.Equal(NotificationStatus.Succeed, streamHelper.Notifications[1].Status);
        }

        [Fact]
        public async Task UpdateInfo_SendDuplicateTitleUpdateCommand_NotificationReturnedWithWarning()
        {
            var title = $"My title {DateTime.UtcNow.Ticks}";
            var id1 = await CreateAsync("Emad", title);
            var id2 = await CreateAsync("Emad");
            await Task.Delay(5000);

            var streamHelper = new NotificationsStreamHelper(_factory, "Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var input = new UpdateInfoTaskInput()
            {
                Note = "New note",
                Title = title
            };
            var response = await client.PutJsonAsync<InputResponse>($"api/todo-tasks/{id2}/update-info", input);

            await Task.Delay(8000);

            Assert.Contains(streamHelper.Notifications, n => n.Status == NotificationStatus.Warning);
        }

        [Fact]
        public async Task UpdateInfo_SendValidCommandFromOtherUser_NoNotificationReturned()
        {
            var streamHelper = new NotificationsStreamHelper(_factory, "Emad");
            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClientWithUser("Yhwach");

            var input = new UpdateInfoTaskInput()
            {
                Note = "New note",
                Title = $"My new title {DateTime.UtcNow.Ticks}"
            };
            await client.PutJsonAsync<InputResponse>($"api/todo-tasks/{id}/update-info", input);

            await Task.Delay(8000);

            Assert.Empty(streamHelper.Notifications);
        }


        private async Task<string> CreateAsync(string user, string title = "any title")
        {
            var client = _factory.CreateClientWithUser(user);

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = title
            };

            var response = await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);

            return response?.Id ?? throw new InvalidOperationException();
        }
    }
}
