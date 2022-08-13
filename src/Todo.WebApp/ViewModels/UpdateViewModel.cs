using Todo.WebApp.Models;

namespace Todo.WebApp.ViewModels
{
    public class UpdateViewModel
    {

        private string? _title;
        public string? Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    TitleChanged(this, _title);
                }
            }
        }
        public string? Note { get; set; }
        public bool DuplicateTitle { get; set; }
        public DateTime? DueDate { get; set; }
        public string? DueDateString
        {
            get => DueDate?.ToString("yyyy-MM-dd");
            set { }
        }

        public event EventHandler<string?> TitleChanged = delegate { };
        public void SetValues(TodoTaskOutput output)
        {
            Title = output.Title;
            Note = output.Note;
            DuplicateTitle = output.DuplicateTitle;
            DueDate = output.DueDate;
        }

        public UpdateTaskInfoInput ToUpdateInfoInput() =>
            new()
            {
                Title = Title,
                Note = Note,
            };

        public ChangeTaskDueDateInput ToChangeDueDateInput() =>
            new()
            {
                DueDate = DueDate?.ToLocalTime() ?? throw new InvalidOperationException("Due date is null."),
            };
    }
}
