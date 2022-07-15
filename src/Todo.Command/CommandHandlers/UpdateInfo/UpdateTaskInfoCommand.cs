using MediatR;

namespace Todo.Command.CommandHandlers.UpdateInfo
{
    public record UpdateTaskInfoCommand(
        Guid Id,
        string UserId,
        string Title,
        string Note
    ) : IRequest<Guid>, ITodoCommand;
}
