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

        public async Task AppendToStreamAsync(Event @event)
        {
            var document = new Document(@event);
            await _container.CreateItemAsync(document, new PartitionKey(document.AggregateId));
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
