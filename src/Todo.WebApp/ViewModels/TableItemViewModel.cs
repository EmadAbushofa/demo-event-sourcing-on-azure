namespace Todo.WebApp.ViewModels
{
    public class TableItemViewModel
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Note { get; set; }
        public string? DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public bool DuplicateTitle { get; set; }
    }
}
