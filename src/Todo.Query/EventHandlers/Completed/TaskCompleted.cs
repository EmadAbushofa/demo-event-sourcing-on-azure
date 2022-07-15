using MediatR;

namespace Todo.Query.EventHandlers.Completed
{
    public record TaskCompleted(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        object Data,
        int Version
    ) : Event<object>(AggregateId, Sequence, UserId, DateTime, Data, Version), IRequest<bool>;
}
