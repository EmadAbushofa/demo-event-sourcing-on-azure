using MediatR;

namespace Todo.Command.CommandHandlers.Delete
{
    public record DeleteCommand(
        Guid Id,
        string UserId
    ) : IRequest<Guid>, ITodoCommand;
}
