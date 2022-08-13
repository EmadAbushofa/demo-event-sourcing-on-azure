using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class ChangeDueDateTaskInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ChangeDueDateTaskInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task ChangeDueDate_SendValidCommand_QueryResultSucceed()
        {
            var id = await CreateAsync("Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var input = new ChangeTaskDueDateInput()
            {
                DueDate = DateTime.UtcNow
            };

            var response = await client.PatchJsonAsync<InputResponse>($"api/todo-tasks/{id}/change-due-date", input);

            await Task.Delay(8000);

            var output = await client.GetAsync<TodoTaskOutput>($"api/todo-tasks/{response.Id}");

            AssertEquality.Of(input, output);
        }

        [Fact]
        public async Task ChangeDueDate_SendInvalidCommand_ReturnBadRequest()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var input = new ChangeTaskDueDateInput().ToHttpContent();

            var response = await client.PatchAsync($"api/todo-tasks/{Guid.NewGuid()}/change-due-date", input);

            var result = await response.GetErrorResult();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotEmpty(result.Detail);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task ChangeDueDate_SendInvalidId_ReturnNotFound()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var input = new ChangeTaskDueDateInput()
            {
                DueDate = DateTime.UtcNow,
            }.ToHttpContent();

            var response = await client.PatchAsync($"api/todo-tasks/{Guid.NewGuid()}/change-due-date", input);

            var result = await response.GetErrorResult();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotEmpty(result.Detail);
        }

        [Fact]
        public async Task ChangeDueDate_UpdateAnotherUsersTask_ReturnNotFound()
        {
            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClientWithUser("Emad");

            var input = new ChangeTaskDueDateInput()
            {
                DueDate = DateTime.UtcNow,
            }.ToHttpContent();

            var response = await client.PatchAsync($"api/todo-tasks/{id}/change-due-date", input);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ChangeDueDate_UnauthorizedRequestSent_ReturnUnauthorized()
        {
            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClient();

            var input = new ChangeTaskDueDateInput()
            {
                DueDate = DateTime.UtcNow,
            }.ToHttpContent();

            var response = await client.PatchAsync($"api/todo-tasks/{id}/change-due-date", input);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<string> CreateAsync(string user)
        {
            var client = _factory.CreateClientWithUser(user);

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = $"My title {DateTime.UtcNow.Ticks}"
            };

            var response = await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);

            return response?.Id ?? throw new InvalidOperationException();
        }
    }
}