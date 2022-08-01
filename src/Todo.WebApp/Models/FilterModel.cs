
namespace Todo.WebApp.Models
{
    public class FilterModel
    {
        public int? Page { get; set; }
        public int? Size { get; set; }
        public bool? IsCompleted { get; set; }
        public string? DueDateFrom { get; set; }
        public string? DueDateTo { get; set; }
    }
}
