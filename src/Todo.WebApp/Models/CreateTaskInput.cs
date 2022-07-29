namespace Todo.WebApp.Models
{
    public class CreateTaskInput
    {
        public string? Title { get; set; }
        public DateTime DueDate { get; set; }
        public string? Note { get; set; }
    }
}
