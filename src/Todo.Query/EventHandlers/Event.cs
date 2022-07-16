namespace Todo.Query.EventHandlers
{
    public record Event<T>(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        T Data,
        int Version
    ) : IEvent;
}
