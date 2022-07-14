using MediatR;

namespace Todo.Command.CommandHandlers.ChangeDueDate
{
    public record ChangeDueDateCommand(
        Guid Id,
        string UserId,
        DateOnly DueDate
    ) : IRequest<Guid>;
}
