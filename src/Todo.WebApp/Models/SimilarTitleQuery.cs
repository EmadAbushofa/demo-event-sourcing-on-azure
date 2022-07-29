namespace Todo.WebApp.Models
{
    public class SimilarTitleQuery
    {
        public string? Title { get; set; }
        public Guid? ExcludedId { get; set; }
    }
}
