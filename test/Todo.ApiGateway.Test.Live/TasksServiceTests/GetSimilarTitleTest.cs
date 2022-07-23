using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class GetSimilarTitleTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public GetSimilarTitleTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task GetSimilarTitle_QueryExistingTitle_ReturnExists()
        {
            var title = $"My title {Guid.NewGuid()}";
            var id = await CreateAsync("Emad", title);

            await Task.Delay(5000);

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync<SimilarTitleOutput>($"api/todo-tasks/similar-title-exists?title={title}");

            Assert.Equal(id, response.Id);
            Assert.True(response.Exists);
        }

        [Fact]
        public async Task GetSimilarTitle_QueryExistingTitleWithExludingTheId_ReturnNotExists()
        {
            var title = $"My title {Guid.NewGuid()}";
            var id = await CreateAsync("Emad", title);

            await Task.Delay(5000);

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync<SimilarTitleOutput>($"api/todo-tasks/similar-title-exists?title={title}&excludedId={id}");

            Assert.False(response.Exists);
            Assert.Null(response.Id);
        }

        [Fact]
        public async Task GetSimilarTitle_QueryNonExistingTitle_ReturnNotExists()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync<SimilarTitleOutput>($"api/todo-tasks/similar-title-exists?title={Guid.NewGuid()}");

            Assert.Null(response.Id);
            Assert.False(response.Exists);
        }

        [Fact]
        public async Task GetSimilarTitle_SendInvalidTitle_ReturnBadRequest()
        {
            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync($"api/todo-tasks/similar-title-exists?excludedId=abc");

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetSimilarTitle_QueryExistingTitleOfAnotherUser_ReturnNotExists()
        {
            var title = $"My title {Guid.NewGuid()}";
            await CreateAsync("Yhwach", title);

            await Task.Delay(5000);

            var client = _factory.CreateClientWithUser("Emad");

            var response = await client.GetAsync<SimilarTitleOutput>($"api/todo-tasks/similar-title-exists?title={title}");

            Assert.Null(response.Id);
            Assert.False(response.Exists);
        }

        [Fact]
        public async Task GetSimilarTitle_UnauthorizedRequestSent_ReturnUnauthorized()
        {
            var title = $"My title {Guid.NewGuid()}";
            await CreateAsync("Yhwach", title);

            await Task.Delay(5000);

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"api/todo-tasks/similar-title-exists?title={title}");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<string> CreateAsync(string user, string title)
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