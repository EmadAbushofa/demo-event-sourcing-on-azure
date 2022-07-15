using Microsoft.AspNetCore.Mvc.Testing;
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

            Assert.All(todoTasks, task =>
            {
                var output = response.Tasks.SingleOrDefault(t => t.Id == task.Id.ToString());
                AssertEquality.OfTaskAndFilterOutput(task, output);
            });
        }
    }
}
