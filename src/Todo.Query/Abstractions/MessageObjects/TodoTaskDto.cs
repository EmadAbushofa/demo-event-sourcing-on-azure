using Todo.Query.Entities;

namespace Todo.Query.Infrastructure.Abstractions.MessageObjects
{
    public class TodoTaskDto
    {
        public TodoTaskDto()
        {

        }

        public TodoTaskDto(TodoTask todoTask)
        {
            Id = todoTask.Id;
            UserId = todoTask.UserId;
            Title = todoTask.Title;
            IsUniqueTitle = todoTask.IsUniqueTitle;
            CreatedAt = todoTask.CreatedAt;
            LastUpdate = todoTask.LastUpdate;
            DueDate = todoTask.DueDate;
            IsCompleted = todoTask.IsCompleted;
            Note = todoTask.Note;
        }

        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public bool IsUniqueTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsCompleted { get; set; }
        public string? Note { get; set; }
    }
}
