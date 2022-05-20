using Todo.Command.Events;

namespace Todo.Command.Abstraction
{
    public interface IEventStore
    {
        Task AppendToStreamAsync(Event @event);
        Task<List<Event>> GetStreamAsync(Guid aggregateId);
        Task<List<Event>> GetStreamAsync(string aggregateId);
    }
}
