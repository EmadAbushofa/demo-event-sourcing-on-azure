using MediatR;

namespace Todo.Query.QueryHandlers.Filter
{
    public record FilterQuery(
        int Page,
        int Size,
        bool? IsCompleted,
        string? UserId,
        DateTime? DueDateFrom,
        DateTime? DueDateTo
    ) : IRequest<FilterResult>
    {
        public int Skip => (Page - 1) * Size;
    }
}
