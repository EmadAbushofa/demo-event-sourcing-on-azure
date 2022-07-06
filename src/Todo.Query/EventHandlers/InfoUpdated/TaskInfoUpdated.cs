using MediatR;

namespace Todo.Query.EventHandlers.InfoUpdated
{
    public record TaskInfoUpdated(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        TaskInfoUpdatedData Data,
        int Version
    ) : Event<TaskInfoUpdatedData>(AggregateId, Sequence, UserId, DateTime, Data, Version), IRequest<bool>;
}
