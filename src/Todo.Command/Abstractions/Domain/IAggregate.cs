using Todo.Command.Events;

namespace Todo.Command.Abstractions.Domain
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Sequence { get; }
        IReadOnlyList<Event> GetUncommittedEvents();
        void MarkChangesAsCommitted();
    }
}
