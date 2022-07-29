using Todo.WebApp.Models;

namespace Todo.WebApp.ViewModels
{
    public class CreateViewModel
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Note { get; set; }
        public DateTime? DueDate { get; set; }

        public CreateTaskInput ToInput() =>
            new()
            {
                Title = Title,
                DueDate = DueDate ?? throw new InvalidOperationException("Due date is null."),
                Note = Note,
            };
    }
}
