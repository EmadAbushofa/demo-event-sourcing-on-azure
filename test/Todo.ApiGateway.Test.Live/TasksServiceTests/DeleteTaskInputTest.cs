using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class DeleteTaskInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public DeleteTaskInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task Delete_SendValidCommand_QueryResultSucceed()
        {
            var id = await CreateAsync("Emad");

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.DeleteAsync($"api/todo-tasks/{id}");

            response.EnsureSuccessStatusCode();

            await Task.Delay(8000);

            var getResponse = await client.GetAsync($"api/todo-tasks/{id}");

            var result = await getResponse.GetErrorResult();

            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
            Assert.NotEmpty(result.Detail);
        }

        [Fact]
        public async Task Delete_SendInvalidId_ReturnNotFound()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.DeleteAsync($"api/todo-tasks/{Guid.NewGuid()}");

            var result = await response.GetErrorResult();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotEmpty(result.Detail);
        }

        [Fact]
        public async Task Delete_DeleteAnotherUsersTask_ReturnNotFound()
        {
            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.DeleteAsync($"api/todo-tasks/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_UnauthorizedRequestSent_ReturnUnauthorized()
        {
            var id = await CreateAsync("Yhwach");

            var client = _factory.CreateClient();

            var response = await client.DeleteAsync($"api/todo-tasks/{id}");

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