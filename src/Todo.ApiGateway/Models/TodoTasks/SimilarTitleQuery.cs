namespace Todo.ApiGateway.Models.TodoTasks
{
    public class SimilarTitleQuery
    {
        public string? Title { get; set; }
        public Guid? ExcludedId { get; set; }
    }
}
