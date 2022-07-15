using MediatR;

namespace Todo.Command.CommandHandlers.Complete
{
    public record CompleteCommand(
        Guid Id,
        string UserId
    ) : IRequest<Guid>;
}
