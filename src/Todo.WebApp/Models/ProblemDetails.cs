namespace Todo.WebApp.Models
{
    public class ProblemDetails
    {
        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
    }
}
