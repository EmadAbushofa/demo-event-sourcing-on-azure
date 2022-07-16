
namespace Todo.ApiGateway.Models.TodoTasks
{
    public class FilterModel
    {
        public int? Page { get; set; }
        public int? Size { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? DueDateFrom { get; set; }
        public DateTime? DueDateTo { get; set; }
    }
}
