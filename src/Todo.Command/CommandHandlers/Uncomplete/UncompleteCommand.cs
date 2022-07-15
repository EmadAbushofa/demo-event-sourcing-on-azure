using MediatR;

namespace Todo.Command.CommandHandlers.Uncomplete
{
    public record UncompleteCommand(
        Guid Id,
        string UserId
    ) : IRequest<Guid>, ITodoCommand;
}
