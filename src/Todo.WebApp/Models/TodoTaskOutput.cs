namespace Todo.WebApp.Models
{
    public class TodoTaskOutput
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Note { get; set; }
        public bool DuplicateTitle { get; set; }
    }
}
