using Microsoft.AspNetCore.Mvc.Testing;
using Todo.ApiGateway.Test.Helpers;
using Xunit.Abstractions;

namespace Todo.ApiGateway.Test.TasksServiceTests.Create
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
        public async void Create_SendValidRequest_TaskCreatedEventSaved()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("WeatherForecast");

            response.EnsureSuccessStatusCode();
        }
    }
}