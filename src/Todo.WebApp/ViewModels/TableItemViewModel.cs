using Todo.WebApp.Models;

namespace Todo.WebApp.ViewModels
{
    public class TableItemViewModel
    {
        public TableItemViewModel(TodoTaskFilterOutput output)
        {
            Id = output.Id;
            Title = output.Title;
            DueDate = output.DueDate.ToString("yyyy-MM-dd");
            IsCompleted = output.IsCompleted;
            DuplicateTitle = output.DuplicateTitle;
        }

        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool DuplicateTitle { get; set; }
    }
}
