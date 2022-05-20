namespace Todo.Command.Infrastructure.Query
{
    public interface ITodoQuery
    {
        Task<bool> SimilarTitleExistsAsync(string title);
    }
}
