using Todo.Command.CommandHandlers.Create;
using Todo.Command.CommandHandlers.UpdateInfo;
using Todo.Command.Server.TodoProto;

namespace Todo.Command.Extensions
{
    public static class CommandsExtensions
    {
        public static CreateTaskCommand ToCommand(this CreateRequest request)
            => new(
                UserId: request.UserId,
                Title: request.Title,
                DueDate: request.DueDate.ToDateTime(),
                Note: request.Note
            );

        public static UpdateTaskInfoCommand ToCommand(this UpdateInfoRequest request)
            => new(
                Id: Guid.Parse(request.Id),
                UserId: request.UserId,
                Title: request.Title,
                Note: request.Note
            );
    }
}
