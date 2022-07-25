using Microsoft.AspNetCore.Mvc.Testing;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Todo.ApiGateway.Test.Live.TodoProto.Channel;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.ChannelTests
{
    public class ConnectionTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ConnectionTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task Notification_ConnectTwice_NotificationReturnedInEachConnection()
        {
            var streamHelper1 = new NotificationsStreamHelper(_factory, "Emad");
            var streamHelper2 = new NotificationsStreamHelper(_factory, "Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = $"My title {DateTime.UtcNow.Ticks}"
            };

            var response = await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);
            await Task.Delay(8000);

            AssertReceived(streamHelper1, response);
            AssertReceived(streamHelper2, response);
        }

        [Fact]
        public async Task Notification_ConnectTwiceWithSameConnectionId_SecondConnectionFails()
        {
            var connectionId = Guid.NewGuid();
            var streamHelper1 = new NotificationsStreamHelper(_factory, "Emad", connectionId);
            var streamHelper2 = new NotificationsStreamHelper(_factory, "Emad", connectionId);

            var client = _factory.CreateClientWithUser("Emad");

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = $"My title {DateTime.UtcNow.Ticks}"
            };

            await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);
            await Task.Delay(8000);

            var messages = streamHelper1.Notifications.Union(streamHelper2.Notifications);
            Assert.Single(messages);
        }

        [Fact]
        public async Task Notification_ConnectTwice2UsersWithSameConnectionId_EachUserGetsHisNotifications()
        {
            var connectionId = Guid.NewGuid();
            var streamHelper1 = new NotificationsStreamHelper(_factory, "Emad", connectionId);
            var streamHelper2 = new NotificationsStreamHelper(_factory, "Yhwach", connectionId);

            var client1 = _factory.CreateClientWithUser("Emad");
            var client2 = _factory.CreateClientWithUser("Yhwach");

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = $"My title {DateTime.UtcNow.Ticks}"
            };

            var response1 = await client1.PostJsonAsync<InputResponse>("api/todo-tasks", input);
            var response2 = await client2.PostJsonAsync<InputResponse>("api/todo-tasks", input);
            await Task.Delay(8000);

            AssertReceived(streamHelper1, response1);
            AssertReceived(streamHelper2, response2);
        }

        private static void AssertReceived(NotificationsStreamHelper streamHelper, InputResponse response)
        {
            Assert.Single(streamHelper.Notifications);
            Assert.NotEmpty(streamHelper.Notifications[0].Message);
            Assert.Equal(response.Id, streamHelper.Notifications[0].TodoTaskId);
            Assert.Equal(NotificationStatus.Succeed, streamHelper.Notifications[0].Status);
        }
    }
}
