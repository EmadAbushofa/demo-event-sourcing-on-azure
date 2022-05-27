using Todo.Command.Abstraction;

namespace Todo.Command.Infrastructure.Query
{
    public class TempTodoQuery : ITodoQuery
    {
        public Task<bool> SimilarTitleExistsAsync(string title)
        {
            return Task.FromResult(false);
        }
    }
}
