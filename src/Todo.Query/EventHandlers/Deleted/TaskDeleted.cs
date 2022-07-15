using MediatR;

namespace Todo.Query.EventHandlers.Deleted
{
    public record TaskDeleted(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        object Data,
        int Version
    ) : Event<object>(AggregateId, Sequence, UserId, DateTime, Data, Version), IRequest<bool>;
}
