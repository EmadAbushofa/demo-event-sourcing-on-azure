namespace Todo.ApiGateway.Models.TodoTasks
{
    public class TodoTaskFilterOutput
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool DuplicateTitle { get; set; }
    }
}
