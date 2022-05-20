using Todo.Command.Features.Create;
using Todo.Command.Server.TodoProto;

namespace Todo.Command.Extensions
{
    public static class CommandsExtensions
    {
        public static CreateTaskCommand ToCommand(this CreateRequest request)
            => new CreateTaskCommand(
                UserId: request.UserId,
                Title: request.Title,
                DueDate: request.DueDate.ToDateTime(),
                Note: request.Note
            );
    }
}
