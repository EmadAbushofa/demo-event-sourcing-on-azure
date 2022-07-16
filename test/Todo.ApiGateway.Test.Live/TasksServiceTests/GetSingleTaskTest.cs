using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class GetSingleTaskTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GetSingleTaskTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task GetSingle_SendValidQuery_ResultSucceed()
        {
            var id = await CreateAsync("Emad");

            await Task.Delay(5000);

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync($"api/todo-tasks/{id}");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetSingle_SendInvalidId_ReturnNotFound()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync("api/todo-tasks/randomfakeid");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSingle_SendNonExistedId_ReturnNotFound()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync($"api/todo-tasks/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSingle_RequestAnotherUsersTask_ReturnNotFound()
        {
            var id = await CreateAsync("Yhwach");

            await Task.Delay(5000);

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync($"api/todo-tasks/{id}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetSingle_UnauthorizedRequestSent_ReturnUnauthorized()
        {
            var id = await CreateAsync("Yhwach");

            await Task.Delay(5000);

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/todo-tasks/{id}");

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