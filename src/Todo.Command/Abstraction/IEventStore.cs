using Todo.Command.Events;

namespace Todo.Command.Abstraction
{
    public interface IEventStore
    {
        Task AppendToStreamAsync(IEnumerable<Event> @events, Guid aggregateId);
        Task<List<Event>> GetStreamAsync(Guid aggregateId);
        Task<List<Event>> GetStreamAsync(string aggregateId);
    }
}
