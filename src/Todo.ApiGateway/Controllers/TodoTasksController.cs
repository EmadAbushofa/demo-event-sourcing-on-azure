using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Todo.ApiGateway.Extensions;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.TodoProto.Command;

namespace Todo.ApiGateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class TodoTasksController : ControllerBase
    {
        private readonly Tasks.TasksClient _commandClient;

        public TodoTasksController(Tasks.TasksClient commandClient)
        {
            _commandClient = commandClient;
        }


        [HttpPost]
        public async Task<InputResponse> CreateAsync([FromBody] CreateTaskInput input)
        {
            var request = input.ToRequest(User);

            var response = await _commandClient.CreateAsync(request);

            return new InputResponse() { Id = response.Id };
        }
    }
}