using Todo.Query.EventHandlers.Created;

namespace Todo.Query.Entities
{
    public class TodoTask
    {
        private TodoTask(
            Guid id,
            string userId,
            string title,
            string normalizedTitle,
            string actualTitle,
            bool isUniqueTitle,
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
            NormalizedTitle = normalizedTitle;
            ActualTitle = actualTitle;
            IsUniqueTitle = isUniqueTitle;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
            DueDate = dueDate;
            IsCompleted = isCompleted;
            Note = note;
        }

        public static TodoTask FromCreatedEvent(TaskCreatedEvent @event, bool isUniqueTitle = true)
        {
            var title = isUniqueTitle ? @event.Data.Title : ToUniqueTitle(@event.Data.Title);

            return new TodoTask(
                id: @event.AggregateId,
                userId: @event.UserId,
                title: title,
                normalizedTitle: title.ToUpper(),
                actualTitle: @event.Data.Title,
                isUniqueTitle: isUniqueTitle,
                createdAt: @event.DateTime,
                lastUpdate: @event.DateTime,
                dueDate: @event.Data.DueDate,
                isCompleted: false,
                note: @event.Data.Note
            );
        }

        private static string ToUniqueTitle(string title)
        {
            var random = new Random();
            return title + "_Copy:" + random.Next(9999).ToString().PadLeft(4, '0');
        }

        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public string Title { get; private set; }
        public string NormalizedTitle { get; private set; }
        public string ActualTitle { get; private set; }
        public bool IsUniqueTitle { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public DateTime DueDate { get; private set; }
        public bool IsCompleted { get; private set; }
        public string? Note { get; private set; }
    }
}
