using MediatR;
using Todo.Query.Entities;

namespace Todo.Query.QueryHandlers.Find
{
    public record FindQuery(
        Guid Id
    ) : IRequest<TodoTask>;
}
