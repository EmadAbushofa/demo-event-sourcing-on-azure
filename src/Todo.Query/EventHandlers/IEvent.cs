namespace Todo.Query.EventHandlers
{
    public interface IEvent
    {
        Guid AggregateId { get; }
        int Sequence { get; }
        string UserId { get; }
        DateTime DateTime { get; }
        int Version { get; }
        string GetDataAsJson();
    }
}
