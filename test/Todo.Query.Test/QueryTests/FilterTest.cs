using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Entities;
using Todo.Query.Test.Client.TodoProto;
using Todo.Query.Test.Fakers;
using Todo.Query.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.Query.Test.QueryTests
{
    public class FilterTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly DbContextHelper _dbContextHelper;

        public FilterTest(WebApplicationFactory<Program> factory, ITestOutputHelper helper)
        {
            _factory = factory.WithDefaultConfigurations(helper, services =>
            {
                services.ReplaceWithInMemoryDatabase();
            });
            _dbContextHelper = new DbContextHelper(_factory.Services);
        }

        [Fact]
        public async Task Filter_QueryExistingEntities_ReturnAll()
        {
            var todoTasks = await _dbContextHelper.InsertAsync(new TodoTaskFaker().Generate(3));

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var response = await client.FilterAsync(new FilterRequest());

            AssertEquality.OfExpectationAndFilterResponse(
                expectedPage: 1,
                expectedSize: 25,
                expectedTotal: 3,
                expectedTasks: 3,
                response: response
            );
            AssertEquality.OfTasksAndFilterOutputs(todoTasks, response);
        }

        [Fact]
        public async Task Filter_QueryWithnNoEntities_ReturnEmptyList()
        {
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var response = await client.FilterAsync(new FilterRequest());

            AssertEquality.OfExpectationAndFilterResponse(
                expectedPage: 1,
                expectedSize: 25,
                expectedTotal: 0,
                expectedTasks: 0,
                response: response
            );
        }

        [Fact]
        public async Task Filter_FilterSpecificUserId_ReturnOnlySelectedUserTasks()
        {
            var emadtasks = new TodoTaskFaker()
                .RuleFor(e => e.UserId, "Emad")
                .Generate(3);

            var othersTasks = new TodoTaskFaker().Generate(4);

            emadtasks = await _dbContextHelper.InsertAsync(emadtasks);
            await _dbContextHelper.InsertAsync(othersTasks);

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var response = await client.FilterAsync(new FilterRequest()
            {
                UserId = "Emad"
            });

            AssertEquality.OfExpectationAndFilterResponse(
                expectedPage: 1,
                expectedSize: 25,
                expectedTotal: 3,
                expectedTasks: 3,
                response: response
            );
            AssertEquality.OfTasksAndFilterOutputs(emadtasks, response);
        }

        [Theory]
        [InlineData(true, 3)]
        [InlineData(false, 2)]
        public async Task Filter_FilterBasedOnTaskCompletionStatus_ReturnOnlyAsExpected(
            bool isCompleted,
            int expectedTotal
        )
        {
            var completedTasks = new TodoTaskFaker()
                .RuleFor(e => e.IsCompleted, true)
                .Generate(3);

            var uncompletedTasks = new TodoTaskFaker()
                .RuleFor(e => e.IsCompleted, false)
                .Generate(2);

            await _dbContextHelper.InsertAsync(completedTasks);
            await _dbContextHelper.InsertAsync(uncompletedTasks);

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var response = await client.FilterAsync(new FilterRequest()
            {
                IsCompleted = isCompleted
            });

            AssertEquality.OfExpectationAndFilterResponse(
                expectedPage: 1,
                expectedSize: 25,
                expectedTotal: expectedTotal,
                expectedTasks: expectedTotal,
                response: response
            );

            if (isCompleted)
                AssertEquality.OfTasksAndFilterOutputs(completedTasks, response);
            else
                AssertEquality.OfTasksAndFilterOutputs(uncompletedTasks, response);
        }

        [Theory]
        [InlineData(null, null, "1", "2", "3", "4")]
        [InlineData(1, 25, "1", "2", "3", "4")]
        [InlineData(1, 2, "1", "2")]
        [InlineData(2, 2, "3", "4")]
        [InlineData(3, 2)]
        [InlineData(1, 3, "1", "2", "3")]
        public async Task Filter_UsePagination_ReturnOnlyAsExpected(
            int? page,
            int? size,
            params string[] results
        )
        {
            var task1 = await GenerateAsync();
            var task2 = await GenerateAsync();
            var task3 = await GenerateAsync();
            var task4 = await GenerateAsync();

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var response = await client.FilterAsync(new FilterRequest()
            {
                Page = page,
                Size = size
            });

            AssertEquality.OfExpectationAndFilterResponse(
                expectedPage: page ?? 1,
                expectedSize: size ?? 25,
                expectedTotal: 4,
                expectedTasks: results.Length,
                response: response
            );

            if (results.Contains("1")) Assert.Contains(response.Tasks, r => r.Id == task1.Id.ToString());
            if (results.Contains("2")) Assert.Contains(response.Tasks, r => r.Id == task2.Id.ToString());
            if (results.Contains("3")) Assert.Contains(response.Tasks, r => r.Id == task3.Id.ToString());
            if (results.Contains("4")) Assert.Contains(response.Tasks, r => r.Id == task4.Id.ToString());
        }

        [Theory]
        [InlineData(null, null, 1, 2, 3, 4)]
        [InlineData("2022-03-02", "2022-07-04", 1, 2, 3, 4)]
        [InlineData("2022-03-03", "2022-07-03", 2, 3)]
        [InlineData("2022-06-25", "2022-06-26", 2, 3)]
        [InlineData("2022-06-26", "2022-06-26", 3)]
        [InlineData("2022-06-26", null, 3, 4)]
        [InlineData(null, "2022-06-25", 1, 2)]
        [InlineData("2022-07-04", null, 4)]
        [InlineData("2022-07-05", null)]
        [InlineData(null, "2022-03-01")]
        [InlineData(null, "2022-03-02", 1)]
        public async Task Filter_FilterDueDate_ReturnOnlyAsExpected(
            string? dueDateFrom,
            string? dueDateTo,
            params int[] results
        )
        {
            var task1 = await GenerateOnDueDateAsync("2022-03-02");
            var task2 = await GenerateOnDueDateAsync("2022-06-25");
            var task3 = await GenerateOnDueDateAsync("2022-06-26");
            var task4 = await GenerateOnDueDateAsync("2022-07-04");

            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());

            var response = await client.FilterAsync(new FilterRequest()
            {
                DueDateFrom = ProtoConverters.ToUtcTimestamp(dueDateFrom),
                DueDateTo = ProtoConverters.ToUtcTimestamp(dueDateTo),
            });

            AssertEquality.OfExpectationAndFilterResponse(
                expectedPage: 1,
                expectedSize: 25,
                expectedTotal: results.Length,
                expectedTasks: results.Length,
                response: response
            );

            if (results.Contains(1)) Assert.Contains(response.Tasks, r => r.Id == task1.Id.ToString());
            if (results.Contains(2)) Assert.Contains(response.Tasks, r => r.Id == task2.Id.ToString());
            if (results.Contains(3)) Assert.Contains(response.Tasks, r => r.Id == task3.Id.ToString());
            if (results.Contains(4)) Assert.Contains(response.Tasks, r => r.Id == task4.Id.ToString());
        }

        private Task<TodoTask> GenerateAsync()
        {
            var todoTask = new TodoTaskFaker().Generate();
            return _dbContextHelper.InsertAsync(todoTask);
        }

        private Task<TodoTask> GenerateOnDueDateAsync(string dueDate)
        {
            var todoTask = new TodoTaskFaker()
                .RuleFor(e => e.DueDate, DateTime.Parse(dueDate))
                .Generate();
            return _dbContextHelper.InsertAsync(todoTask);
        }
    }
}
