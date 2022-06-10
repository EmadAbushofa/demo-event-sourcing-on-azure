using Todo.Command.Events;
using Todo.Command.Models;

namespace Todo.Command.Abstractions
{
    public interface IEventStore
    {
        Task AppendToStreamAsync(IAggregate aggregate);
        Task AppendToStreamAsync(Event @event);
        Task<List<Event>> GetStreamAsync(Guid aggregateId);
        Task<List<Event>> GetStreamAsync(string aggregateId);
    }
}
