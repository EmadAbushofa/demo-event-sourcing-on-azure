using Todo.Command.Abstractions.Domain;
using Todo.Command.Abstractions.Persistence;
using Todo.Command.Events;

namespace Todo.Command.Test.FakeServices
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<Event> _events = new();

        public Task AppendToStreamAsync(IAggregate aggregate)
        {
            foreach (var @event in aggregate.GetUncommittedEvents())
            {
                _events.Add(@event);
            }

            return Task.CompletedTask;
        }

        public Task AppendToStreamAsync(Event @event)
        {
            _events.Add(@event);
            return Task.CompletedTask;
        }

        public Task<List<Event>> GetStreamAsync(Guid aggregateId)
        {
            var stream = _events
                .Where(e => e.AggregateId == aggregateId)
                .OrderBy(e => e.Sequence)
                .ToList();

            return Task.FromResult(stream);
        }

        public Task<List<Event>> GetStreamAsync(string aggregateId)
            => GetStreamAsync(Guid.Parse(aggregateId));
    }
}
