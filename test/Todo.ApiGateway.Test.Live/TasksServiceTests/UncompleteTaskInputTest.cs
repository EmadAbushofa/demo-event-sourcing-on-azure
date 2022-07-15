using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class UncompleteTaskInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UncompleteTaskInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task Uncomplete_SendValidCommand_QueryResultSucceed()
        {
            var id = await CreateAsync("Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.PatchJsonAsync<InputResponse>($"api/todo-tasks/{id}/uncomplete");

            await Task.Delay(8000);

            var output = await client.GetAsync<TodoTaskOutput>($"api/todo-tasks/{response.Id}");

            Assert.NotNull(output);
            Assert.False(output.IsCompleted);
        }

        [Fact]
        public async Task Uncomplete_SendInvalidId_ReturnNotFound()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.PatchAsync($"api/todo-tasks/{Guid.NewGuid()}/uncomplete", null);

            var result = await response.GetErrorResult();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotEmpty(result.Detail);
        }

        [Fact]
        public async Task Uncomplete_UncompleteAnotherUsersTask_ReturnNotFound()
        {
            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.PatchAsync($"api/todo-tasks/{id}/uncomplete", null);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Uncomplete_UnauthorizedRequestSent_ReturnUnauthorized()
        {
            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClient();

            var response = await client.PatchAsync($"api/todo-tasks/{id}/uncomplete", null);

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
            await client.PatchAsync($"api/todo-tasks/{response.Id}/complete", null);

            return response?.Id ?? throw new InvalidOperationException();
        }
    }
}