using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.Test.Helpers;
using Todo.ApiGateway.Test.Live.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.Live.TasksServiceTests
{
    public class FilterTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public FilterTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.UseTestAuthentication();
            });
        }

        [Fact]
        public async Task Filter_WithExistingRecord_ReturnUserSpecificResults()
        {
            var emad = Guid.NewGuid().ToString();
            var id = await CreateAsync(emad);

            await Task.Delay(8000);

            var client = _factory.CreateClientWithUser(emad.ToString());

            var result = await client.GetAsync<FilterResult>($"api/todo-tasks");

            AssertEquality.Of(result, id);
        }

        [Theory]
        [InlineData(true, 0)]
        [InlineData(false, 1)]
        public async Task Filter_FilterWithCompletionStatus_ReturnAsExpected(bool isCompleted, int expectedCount)
        {
            var emad = Guid.NewGuid().ToString();
            await CreateAsync(emad);

            await Task.Delay(8000);

            var client = _factory.CreateClientWithUser(emad);

            var result = await client.GetAsync<FilterResult>($"api/todo-tasks?isCompleted={isCompleted}");

            Assert.Equal(expectedCount, result.Tasks.Count);
        }

        [Theory]
        [InlineData(2, 25)]
        [InlineData(1, 10)]
        public async Task Filter_FilterWithPagination_ReturnAsExpected(int page, int size)
        {
            var emad = Guid.NewGuid().ToString();
            await CreateAsync(emad);

            await Task.Delay(8000);

            var client = _factory.CreateClientWithUser(emad);

            var result = await client.GetAsync<FilterResult>($"api/todo-tasks?page={page}&size={size}");

            Assert.Equal(page, result.Page);
            Assert.Equal(size, result.Size);
            Assert.Equal(1, result.Total);
        }

        [Theory]
        [InlineData("2022-05-21", null, 0)]
        [InlineData(null, "2022-05-19", 0)]
        [InlineData("2022-05-19", "2022-05-21", 1)]
        public async Task Filter_FilterWithDueDate_ReturnAsExpected(string? dateFrom, string? dateTo, int count)
        {
            var emad = Guid.NewGuid().ToString();
            await CreateAsync(emad, "2022-05-20");

            await Task.Delay(8000);

            var client = _factory.CreateClientWithUser(emad);

            var result = await client.GetAsync<FilterResult>($"api/todo-tasks?dueDateFrom={dateFrom}&dueDateTo={dateTo}");

            Assert.Equal(count, result.Tasks.Count);
        }

        [Fact]
        public async Task Filter_WithExistingRecordOfOtherUser_ReturnEmpty()
        {
            var emad = Guid.NewGuid().ToString();
            var yhwach = Guid.NewGuid().ToString();

            await CreateAsync(yhwach);

            await Task.Delay(8000);

            var client = _factory.CreateClientWithUser(emad);

            var result = await client.GetAsync<FilterResult>("api/todo-tasks");

            Assert.Empty(result.Tasks);
        }

        [Fact]
        public async Task Filter_UnauthorizedRequestSent_ReturnUnauthorized()
        {
            var emad = Guid.NewGuid().ToString();
            await CreateAsync(emad);

            await Task.Delay(8000);

            var client = _factory.CreateClient();

            var response = await client.GetAsync("api/todo-tasks");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private async Task<string> CreateAsync(string userId, string? dueDateString = null)
        {
            var client = _factory.CreateClientWithUser(userId);

            var input = new CreateTaskInput()
            {
                DueDate = dueDateString == null ? DateTime.UtcNow : DateTime.Parse(dueDateString),
                Note = "Some note",
                Title = "My title " + Guid.NewGuid()
            };

            var response = await client.PostJsonAsync<InputResponse>("api/todo-tasks", input);

            return response?.Id ?? throw new InvalidOperationException();
        }
    }
}