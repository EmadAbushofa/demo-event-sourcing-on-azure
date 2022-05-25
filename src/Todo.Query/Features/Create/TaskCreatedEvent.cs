using MediatR;
using Todo.Query.Enums;

namespace Todo.Query.Features.Create
{
    public record TaskCreatedEvent(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        TaskCreatedData Data,
        int Version
    ) : Event<TaskCreatedData>(AggregateId, Sequence, UserId, DateTime, Data, EventType.TaskCreated, Version), IRequest<bool>
    {

    }
}
