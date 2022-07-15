using Todo.Query.Entities;

namespace Todo.Query.QueryHandlers.Filter
{
    public record FilterResult(
        int Page,
        int Size,
        int Total,
        List<TodoTask> Tasks
    );
}
