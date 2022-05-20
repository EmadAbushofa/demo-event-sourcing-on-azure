namespace Todo.Command.Abstraction
{
    public interface ITodoQuery
    {
        Task<bool> SimilarTitleExistsAsync(string title);
    }
}
