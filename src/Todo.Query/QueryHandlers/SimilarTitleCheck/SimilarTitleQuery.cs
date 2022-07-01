using MediatR;
using Todo.Query.Entities;

namespace Todo.Query.QueryHandlers.SimilarTitleCheck
{
    public record SimilarTitleQuery(
        string UserId,
        string Title
    ) : IRequest<TodoTask?>;
}
