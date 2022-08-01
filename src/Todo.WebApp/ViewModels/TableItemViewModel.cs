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
            State = output.IsCompleted ? "Uncomplete" : "Complete";
            DuplicateTitle = output.DuplicateTitle;
        }

        public TableItemViewModel(TodoTaskOutput output)
        {
            Id = output.Id;
            Title = output.Title;
            DueDate = output.DueDate.ToString("yyyy-MM-dd");
            State = output.IsCompleted ? "Uncomplete" : "Complete";
            DuplicateTitle = output.DuplicateTitle;
        }

        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? DueDate { get; set; }
        public string State { get; set; }
        public bool DuplicateTitle { get; set; }

        public void Update(TodoTaskOutput output)
        {
            Title = output.Title;
            DueDate = output.DueDate.ToString("yyyy-MM-dd");
            State = output.IsCompleted ? "Uncomplete" : "Complete";
            DuplicateTitle = output.DuplicateTitle;
        }
    }
}
