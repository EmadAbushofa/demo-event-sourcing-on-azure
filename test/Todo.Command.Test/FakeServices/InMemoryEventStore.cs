using Todo.Command.Abstraction;
using Todo.Command.Events;

namespace Todo.Command.Test.FakeServices
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly List<Event> _events = new();

        public Task AppendToStreamAsync(IEnumerable<Event> events, Guid aggregateId)
        {
            var partitionKey = aggregateId.ToString();

            foreach (var @event in events)
            {
                if (@event.AggregateId.ToString() != partitionKey)
                    throw new ArgumentException("Not all events have the same aggregateId.", nameof(events));

                _events.Add(@event);

                var sequenceCount = _events.Count(e => e.AggregateId == @event.AggregateId && e.Sequence == @event.Sequence);

                if (sequenceCount != 1)
                    throw new InvalidOperationException("Duplicate sequence found in stream.");
            }

            return Task.CompletedTask;
        }

        public Task<List<Event>> GetStreamAsync(Guid aggregateId)
        {
            var stream = _events.Where(e => e.AggregateId == aggregateId).ToList();

            return Task.FromResult(stream);
        }

        public Task<List<Event>> GetStreamAsync(string aggregateId)
            => GetStreamAsync(Guid.Parse(aggregateId));
    }
}
