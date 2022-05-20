using MediatR;

namespace Todo.Command.Features.Create
{
    public record CreateTaskCommand(
        string UserId,
        string Title,
        DateTime DueDate,
        string Note
    ) : IRequest<Guid>;
}
