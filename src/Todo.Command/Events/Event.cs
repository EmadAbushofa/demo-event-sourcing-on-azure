namespace Todo.Command.Events
{
    public abstract class Event
    {
        protected Event(
            Guid aggregateId,
            int sequence,
            string userId,
            EventType type
        )
        {
            AggregateId = aggregateId;
            UserId = userId;
            Sequence = sequence;
            DateTime = DateTime.UtcNow;
            Type = type;
            Version = 1;
        }

        public Guid AggregateId { get; protected set; }
        public int Sequence { get; protected set; }
        public string UserId { get; protected set; }
        public DateTime DateTime { get; protected set; }
        public EventType Type { get; protected set; }
        public int Version { get; protected set; }
    }

    public abstract class Event<T> : Event
    {
        protected Event(
            Guid aggregateId,
            int sequence,
            T data,
            string userId,
            EventType type
        ) : base(aggregateId, sequence, userId, type)
        {
            Data = data;
        }

        public T Data { get; protected set; }
    }
}
