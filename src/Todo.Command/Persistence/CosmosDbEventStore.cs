using Microsoft.Azure.Cosmos;
using Todo.Command.Abstraction;
using Todo.Command.Events;

namespace Todo.Command.Persistence
{
    public partial class CosmosDbEventStore : IEventStore
    {
        private readonly Container _container;

        public CosmosDbEventStore(Container container)
        {
            _container = container;
        }

        public async Task AppendToStreamAsync(IEnumerable<Event> events, Guid aggregateId)
        {
            var partitionKey = aggregateId.ToString();

            var batch = _container.CreateTransactionalBatch(new PartitionKey(partitionKey));

            foreach (var @event in events)
            {
                var document = new Document(@event);

                if (document.AggregateId != partitionKey)
                    throw new ArgumentException("Not all events have the same aggregateId.", nameof(events));

                batch.CreateItem(document);
            }

            await batch.ExecuteAsync();
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
