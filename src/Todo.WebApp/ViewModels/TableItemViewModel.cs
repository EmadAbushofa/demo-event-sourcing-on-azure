using Todo.WebApp.Models;

namespace Todo.WebApp.ViewModels
{
    public class TableItemViewModel
    {
        public TableItemViewModel(TodoTaskFilterOutput output)
        {
            Id = output.Id;
            Title = output.Title;
            DueDate = output.DueDate;
            IsCompleted = output.IsCompleted;
            DuplicateTitle = output.DuplicateTitle;
        }

        public TableItemViewModel(TodoTaskOutput output)
        {
            Id = output.Id;
            Title = output.Title;
            DueDate = output.DueDate;
            IsCompleted = output.IsCompleted;
            DuplicateTitle = output.DuplicateTitle;
        }

        public string? Id { get; set; }
        public string? Title { get; set; }
        public DateTime DueDate { get; set; }
        public string? DueDateString => DueDate.ToString("yyyy-MM-dd");
        public DueDateState DueDateState => (DueDate - DateTime.Today).TotalDays switch
        {
            < 7 and > 0 => DueDateState.Soon,
            0 => DueDateState.Today,
            < 0 => DueDateState.Late,
            _ => DueDateState.Future,
        };
        public bool IsCompleted { get; set; }
        public bool StateIsChanging { get; set; }
        public bool Deleting { get; set; }
        public bool Disabled => Deleting || StateIsChanging;
        public string State => IsCompleted ? "Uncomplete" : "Complete";
        public bool DuplicateTitle { get; set; }

        public void Update(TodoTaskOutput output)
        {
            Title = output.Title;
            DueDate = output.DueDate;
            IsCompleted = output.IsCompleted;
            DuplicateTitle = output.DuplicateTitle;
            StateIsChanging = false;
            Deleting = false;
        }

        public void IsChanging() => StateIsChanging = true;
        public void IsDeleting() => Deleting = true;
    }
}
