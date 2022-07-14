using MediatR;

namespace Todo.Query.EventHandlers.DueDateChanged
{
    public record TaskDueDateChanged(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        TaskDueDateChangedData Data,
        int Version
    ) : Event<TaskDueDateChangedData>(AggregateId, Sequence, UserId, DateTime, Data, Version), IRequest<bool>;
}
