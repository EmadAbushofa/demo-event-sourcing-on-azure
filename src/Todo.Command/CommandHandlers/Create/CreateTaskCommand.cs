using MediatR;

namespace Todo.Command.CommandHandlers.Create
{
    public record CreateTaskCommand(
        string UserId,
        string Title,
        DateOnly DueDate,
        string Note
    ) : IRequest<Guid>;
}
