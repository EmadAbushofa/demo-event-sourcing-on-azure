using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class UpdateInfoTaskInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public UpdateInfoTaskInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task UpdateInfo_SendValidCommand_QueryResultSucceed()
        {
            var id = await CreateAsync();

            var client = _factory.CreateClient();

            var input = new UpdateInfoTaskInput()
            {
                Note = "New note",
                Title = $"My new title {DateTime.UtcNow.Ticks}"
            };

            var response = await client.PutJsonAsync<InputResponse>($"api/todo-tasks/{id}/update-info", input);

            await Task.Delay(5000);

            var output = await client.GetAsync<TodoTaskOutput>($"api/todo-tasks/{response.Id}");

            AssertEquality.Of(input, output);
        }

        [Fact]
        public async Task UpdateInfo_SendInvalidCommand_ReturnBadRequest()
        {
            var client = _factory.CreateClient();

            var input = new UpdateInfoTaskInput().ToHttpContent();

            var response = await client.PutAsync($"api/todo-tasks/{Guid.NewGuid()}/update-info", input);

            var result = await response.GetErrorResult();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotEmpty(result.Detail);
            Assert.NotEmpty(result.Errors);
        }

        private async Task<string> CreateAsync()
        {
            var client = _factory.CreateClient();

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