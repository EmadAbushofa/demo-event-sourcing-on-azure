using Todo.Query.EventHandlers.Created;

namespace Todo.Query.Entities
{
    public class TodoTask
    {
        private TodoTask(
            Guid id,
            string userId,
            string title,
            DateTime createdAt,
            DateTime lastUpdate,
            DateTime dueDate,
            bool isCompleted,
            string note
        )
        {
            Id = id;
            UserId = userId;
            Title = title;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
            DueDate = dueDate;
            IsCompleted = isCompleted;
            Note = note;
        }

        public static TodoTask FromCreatedEvent(TaskCreatedEvent @event)
        {
            return new TodoTask(
                id: @event.AggregateId,
                userId: @event.UserId,
                title: @event.Data.Title,
                createdAt: @event.DateTime,
                lastUpdate: @event.DateTime,
                dueDate: @event.Data.DueDate,
                isCompleted: false,
                note: @event.Data.Note
            );
        }

        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string Title { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public DateTime DueDate { get; private set; }
        public bool IsCompleted { get; private set; }
        public string Note { get; private set; }
    }
}
