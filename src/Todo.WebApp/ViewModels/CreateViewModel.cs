using Todo.WebApp.Models;

namespace Todo.WebApp.ViewModels
{
    public class CreateViewModel
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
        public DateTime? DueDate { get; set; }
        public bool DuplicateTitle { get; set; }

        public event EventHandler<string?> TitleChanged = delegate { };

        public CreateTaskInput ToInput() =>
            new()
            {
                Title = Title,
                DueDate = DueDate?.ToLocalTime() ?? throw new InvalidOperationException("Due date is null."),
                Note = Note,
            };
    }
}
