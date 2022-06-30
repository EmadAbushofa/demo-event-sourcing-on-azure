namespace Todo.ApiGateway.Models.TodoTasks
{
    public class TodoTaskOutput
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Note { get; set; }
    }
}
