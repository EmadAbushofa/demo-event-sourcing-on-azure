using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using Todo.ApiGateway.Extensions;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.TodoProto.Command;
using Todo.ApiGateway.TodoProto.Query;
using CommandClient = Todo.ApiGateway.TodoProto.Command.Tasks.TasksClient;
using QueryClient = Todo.ApiGateway.TodoProto.Query.Tasks.TasksClient;

namespace Todo.ApiGateway.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class TodoTasksController : ControllerBase
    {
        private readonly CommandClient _commandClient;
        private readonly QueryClient _queryClient;

        public TodoTasksController(
            CommandClient commandClient,
            QueryClient queryClient
        )
        {
            _commandClient = commandClient;
            _queryClient = queryClient;
        }

        [HttpGet]
        public async Task<FilterResult> IndexAsync([FromQuery] FilterModel filter)
        {
            var request = filter.ToRequest(User);

            var response = await _queryClient.FilterAsync(request);

            return response.ToOutput();
        }

        [HttpGet("{id}")]
        public async Task<TodoTaskOutput> FindAsync(string id)
        {
            var request = new FindRequest() { Id = id };

            var response = await _queryClient.FindAsync(request);

            return response.ToOutput();
        }

        [HttpGet("similar-title-exists")]
        public async Task<SimilarTitleOutput> SimilarTitleExistsAsync(string? title)
        {
            var request = new SimilarTitleExistsRequest() { Title = title ?? "", UserId = User.GetId() };

            var response = await _queryClient.SimilarTitleExistsAsync(request);

            return response.ToOutput();
        }

        [HttpPost]
        public async Task<InputResponse> CreateAsync([FromBody] CreateTaskInput input)
        {
            var request = input.ToRequest(User);

            var response = await _commandClient.CreateAsync(request);

            return new InputResponse() { Id = response.Id };
        }

        [HttpPut("{id}/update-info")]
        public async Task<InputResponse> UpdateInfoAsync(Guid id, [FromBody] UpdateInfoTaskInput input)
        {
            var request = input.ToRequest(id, User);

            var response = await _commandClient.UpdateInfoAsync(request);

            return new InputResponse() { Id = response.Id };
        }

        [HttpPatch("{id}/change-due-date")]
        public async Task<InputResponse> ChangeDueDateAsync(Guid id, [FromBody] ChangeDueDateTaskInput input)
        {
            var request = input.ToRequest(id, User);

            var response = await _commandClient.ChangeDueDateAsync(request);

            return new InputResponse() { Id = response.Id };
        }

        [HttpPatch("{id}/complete")]
        public async Task<InputResponse> CompleteAsync(Guid id)
        {
            var request = new CompleteRequest()
            {
                Id = id.ToString(),
                UserId = User.GetId(),
            };

            var response = await _commandClient.CompleteAsync(request);

            return new InputResponse() { Id = response.Id };
        }

        [HttpPatch("{id}/uncomplete")]
        public async Task<InputResponse> UncompleteAsync(Guid id)
        {
            var request = new CompleteRequest()
            {
                Id = id.ToString(),
                UserId = User.GetId(),
            };

            var response = await _commandClient.UncompleteAsync(request);

            return new InputResponse() { Id = response.Id };
        }

        [HttpDelete("{id}")]
        public async Task<InputResponse> DeleteAsync(Guid id)
        {
            var request = new DeleteRequest()
            {
                Id = id.ToString(),
                UserId = User.GetId(),
            };

            var response = await _commandClient.DeleteAsync(request);

            return new InputResponse() { Id = response.Id };
        }
    }
}