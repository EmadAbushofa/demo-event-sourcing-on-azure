using Todo.Query.Enums;

namespace Todo.Query.Features
{
    public record Event<T>(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        T Data,
        EventType Type,
        int Version
    );
}
