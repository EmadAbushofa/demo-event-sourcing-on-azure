using Todo.Command.Abstractions.Domain;
using Todo.Command.Events;

namespace Todo.Command.Abstractions.Persistence
{
    public interface IEventStore
    {
        Task AppendToStreamAsync(IAggregate aggregate);
        Task AppendToStreamAsync(Event @event);
        Task<List<Event>> GetStreamAsync(Guid aggregateId);
        Task<List<Event>> GetStreamAsync(string aggregateId);
    }
}
