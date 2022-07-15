using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Todo.Command.Abstractions.Persistence;
using Todo.Command.Events;

namespace Todo.Command.Test.Helpers
{
    public class EventStoreHelper
    {
        private readonly IServiceProvider _provider;

        public EventStoreHelper(IServiceProvider provider)
        {
            _provider = provider;
        }

        public async Task<TEvent> GenerateAndAppendToStreamAsync<TEvent>(Faker<TEvent> faker)
            where TEvent : Event
        {
            var eventStore = _provider.GetRequiredService<IEventStore>();
            var @event = faker.Generate();
            await eventStore.AppendToStreamAsync(@event);
            return @event;
        }
    }
}