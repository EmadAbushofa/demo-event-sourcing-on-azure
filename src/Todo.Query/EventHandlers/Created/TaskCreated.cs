using MediatR;

namespace Todo.Query.EventHandlers.Created
{
    public record TaskCreated(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        TaskCreatedData Data,
        int Version
    ) : Event<TaskCreatedData>(AggregateId, Sequence, UserId, DateTime, Data, Version), IRequest<bool>;
}
