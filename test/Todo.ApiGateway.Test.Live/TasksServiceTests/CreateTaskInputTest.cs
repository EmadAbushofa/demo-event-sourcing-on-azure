using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class CreateTaskInputTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CreateTaskInputTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task Create_SendValidCommand_QueryResultSucceed()
        {
            var client = _factory.CreateClient();

            var input = new CreateTaskInput()
            {
                DueDate = DateTime.UtcNow,
                Note = "Some note",
                Title = $"My title {DateTime.UtcNow.Ticks}"
            };

            var response = await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);

            await Task.Delay(5000);

            var output = await client.GetAsync<TodoTaskOutput>($"api/todo-tasks/{response.Id}");

            AssertEquality.Of(input, output);
        }

        [Fact]
        public async Task Create_SendInvalidCommand_ReturnBadRequest()
        {
            var client = _factory.CreateClient();

            var input = new CreateTaskInput().ToHttpContent();

            var response = await client.PostAsync("api/todo-tasks", input);

            var result = await response.GetErrorResult();

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.NotEmpty(result.Detail);
            Assert.NotEmpty(result.Errors);
        }
    }
}