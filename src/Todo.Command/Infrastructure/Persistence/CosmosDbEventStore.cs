using Microsoft.Azure.Cosmos;
using Todo.Command.Abstractions;
using Todo.Command.Events;
using Todo.Command.Models;

namespace Todo.Command.Infrastructure.Persistence
{
    public partial class CosmosDbEventStore : IEventStore
    {
        private readonly Container _container;

        public CosmosDbEventStore(Container container)
        {
            _container = container;
        }

        public async Task AppendToStreamAsync(IAggregate aggregate)
        {
            await CreateAsync(aggregate.GetUncommittedEvents());

            aggregate.MarkChangesAsCommitted();
        }

        public Task AppendToStreamAsync(Event @event)
            => CreateAsync(new Event[] { @event });

        private Task CreateAsync(IReadOnlyList<Event> events)
        {
            var partitionKey = events[0].AggregateId.ToString();

            var batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            foreach (var @event in events)
            {
                var document = new Document(@event);

                batch.CreateItem(document);
            }

            return batch.ExecuteAsync();
        }

        public Task<List<Event>> GetStreamAsync(string aggregateId)
            => GetStreamAsync(Guid.Parse(aggregateId));

        public async Task<List<Event>> GetStreamAsync(Guid aggregateId)
        {
            var list = new List<Event>();

            var feedIterator = _container.GetItemQueryIterator<Document>($"SELECT * FROM Todos WHERE Todos.aggregateId = '{aggregateId}'");

            while (feedIterator.HasMoreResults)
            {
                var response = await feedIterator.ReadNextAsync();

                var events = response.Select(r => r.ToEvent());

                list.AddRange(events);
            }

            return list;
        }
    }
}
