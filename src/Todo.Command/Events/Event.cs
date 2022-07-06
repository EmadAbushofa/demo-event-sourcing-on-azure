namespace Todo.Command.Events
{
    public abstract class Event
    {
        protected Event(
            Guid aggregateId,
            int sequence,
            string userId
        )
        {
            AggregateId = aggregateId;
            UserId = userId;
            Sequence = sequence;
            DateTime = DateTime.UtcNow;
            Version = 1;
        }

        public Guid AggregateId { get; protected set; }
        public int Sequence { get; protected set; }
        public string UserId { get; protected set; }
        public DateTime DateTime { get; protected set; }
        public int Version { get; protected set; }
        public string Type => GetType().Name;
    }

    public abstract class Event<T> : Event
    {
        protected Event(
            Guid aggregateId,
            int sequence,
            T data,
            string userId
        ) : base(aggregateId, sequence, userId)
        {
            Data = data;
        }

        public T Data { get; protected set; }
    }
}
