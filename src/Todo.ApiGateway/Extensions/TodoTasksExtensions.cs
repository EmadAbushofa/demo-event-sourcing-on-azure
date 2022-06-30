using Google.Protobuf.WellKnownTypes;
using System.Security.Claims;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.TodoProto.Command;

namespace Todo.ApiGateway.Extensions
{
    public static class TodoTasksExtensions
    {
        public static CreateRequest ToRequest(this CreateTaskInput input, ClaimsPrincipal claims)
            => new()
            {
                UserId = claims.GetUserId(),
                DueDate = input.DueDate.ToTimestamp(),
                Note = input.Note,
                Title = input.Title,
            };
    }
}
