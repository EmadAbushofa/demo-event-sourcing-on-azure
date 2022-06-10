using Todo.Command.Events;

namespace Todo.Command.Models
{
    public interface IAggregate
    {
        Guid Id { get; }
        int Sequence { get; }
        IReadOnlyList<Event> GetUncommittedEvents();
        void MarkChangesAsCommitted();
    }
}
