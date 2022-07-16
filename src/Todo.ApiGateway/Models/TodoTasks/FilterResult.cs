namespace Todo.ApiGateway.Models.TodoTasks
{
    public class FilterResult
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int Total { get; set; }
        public List<TodoTaskFilterOutput> Tasks { get; set; } = new();
    }
}
