using MediatR;

namespace Todo.Query.QueryHandlers.Filter
{
    public record FilterQuery(
        int Page,
        int Size,
        bool? IsCompleted,
        string? UserId,
        DateTime? DueDate
    ) : IRequest<FilterResult>;
}
