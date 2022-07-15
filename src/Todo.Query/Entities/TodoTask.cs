using Todo.Query.EventHandlers.Completed;
using Todo.Query.EventHandlers.Created;
using Todo.Query.EventHandlers.DueDateChanged;
using Todo.Query.EventHandlers.InfoUpdated;

namespace Todo.Query.Entities
{
    public class TodoTask
    {
        private TodoTask(
            Guid id,
            int sequence,
            string userId,
            string title,
            string normalizedTitle,
            bool isUniqueTitle,
            DateTime createdAt,
            DateTime lastUpdate,
            DateTime dueDate,
            bool isCompleted,
            string note
        )
        {
            Id = id;
            Sequence = sequence;
            UserId = userId;
            Title = title;
            NormalizedTitle = normalizedTitle;
            IsUniqueTitle = isUniqueTitle;
            CreatedAt = createdAt;
            LastUpdate = lastUpdate;
            DueDate = dueDate;
            IsCompleted = isCompleted;
            Note = note;
        }

        public static TodoTask FromCreatedEvent(TaskCreated @event, bool isUniqueTitle = true)
        {
            return new TodoTask(
                id: @event.AggregateId,
                sequence: @event.Sequence,
                userId: @event.UserId,
                title: @event.Data.Title,
                normalizedTitle: NormalizeTitle(@event.Data.Title, isUniqueTitle),
                isUniqueTitle: isUniqueTitle,
                createdAt: @event.DateTime,
                lastUpdate: @event.DateTime,
                dueDate: @event.Data.DueDate,
                isCompleted: false,
                note: @event.Data.Note
            );
        }

        private static string NormalizeTitle(string title, bool isUnique)
        {
            if (isUnique)
                return title.ToUpper();

            var random = new Random();
            return (title + "_Copy:" + random.Next(9999).ToString().PadLeft(4, '0')).ToUpper();
        }

        public Guid Id { get; private set; }
        public int Sequence { get; private set; }
        public string UserId { get; private set; }
        public string Title { get; private set; }
        public string NormalizedTitle { get; private set; }
        public bool IsUniqueTitle { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public DateTime DueDate { get; private set; }
        public bool IsCompleted { get; private set; }
        public string? Note { get; private set; }

        public void Apply(TaskInfoUpdated @event, bool isUniqueTitle = true)
        {
            Title = @event.Data.Title;
            IsUniqueTitle = isUniqueTitle;
            NormalizedTitle = NormalizeTitle(@event.Data.Title, isUniqueTitle);
            Sequence = @event.Sequence;
            LastUpdate = @event.DateTime;
            Note = @event.Data.Note;
        }

        public void Apply(TaskDueDateChanged @event)
        {
            Sequence = @event.Sequence;
            LastUpdate = @event.DateTime;
            DueDate = @event.Data.DueDate;
        }

        public void Apply(TaskCompleted @event)
        {
            Sequence = @event.Sequence;
            LastUpdate = @event.DateTime;
            IsCompleted = true;
        }
    }
}
