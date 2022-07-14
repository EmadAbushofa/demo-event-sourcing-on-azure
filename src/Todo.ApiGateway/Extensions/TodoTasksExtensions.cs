using Google.Protobuf.WellKnownTypes;
using System.Security.Claims;
using Todo.ApiGateway.Models.TodoTasks;
using Todo.ApiGateway.TodoProto.Command;
using Todo.ApiGateway.TodoProto.Query;

namespace Todo.ApiGateway.Extensions
{
    public static class TodoTasksExtensions
    {
        public static TodoTaskOutput ToOutput(this FindResponse response)
            => new()
            {
                Id = response.Id,
                UserId = response.UserId,
                DueDate = response.DueDate.ToDateTime(),
                Note = response.Note,
                Title = response.Title,
                IsCompleted = response.IsCompleted,
            };

        public static CreateRequest ToRequest(this CreateTaskInput input, ClaimsPrincipal claims)
            => new()
            {
                UserId = claims.GetUserId(),
                DueDate = input.DueDate.ToUniversalTime().ToTimestamp(),
                Note = input.Note,
                Title = input.Title ?? "",
            };

        public static UpdateInfoRequest ToRequest(this UpdateInfoTaskInput input, Guid id, ClaimsPrincipal claims)
            => new()
            {
                Id = id.ToString(),
                UserId = claims.GetUserId(),
                Note = input.Note,
                Title = input.Title ?? "",
            };

        public static ChangeDueDateRequest ToRequest(this ChangeDueDateTaskInput input, Guid id, ClaimsPrincipal claims)
            => new()
            {
                Id = id.ToString(),
                UserId = claims.GetUserId(),
                DueDate = input.DueDate.ToUniversalTime().ToTimestamp(),
            };
    }
}
