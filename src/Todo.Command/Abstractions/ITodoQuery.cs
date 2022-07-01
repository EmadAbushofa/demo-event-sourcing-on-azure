namespace Todo.Command.Abstractions
{
    public interface ITodoQuery
    {
        Task<bool> SimilarTitleExistsAsync(string userId, string title);
    }
}
