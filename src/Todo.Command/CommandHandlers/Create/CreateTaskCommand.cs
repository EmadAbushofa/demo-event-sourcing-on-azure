using MediatR;

namespace Todo.Command.CommandHandlers.Create
{
    public record CreateTaskCommand(
        string UserId,
        string Title,
        DateTime DueDate,
        string Note
    ) : IRequest<Guid>, ITodoCommand;
}
