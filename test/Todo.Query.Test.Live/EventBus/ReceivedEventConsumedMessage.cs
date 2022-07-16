using Newtonsoft.Json;
using Todo.Query.EventHandlers;
using Todo.Query.Infrastructure.Abstractions.MessageObjects;

namespace Todo.Query.Test.Live.EventBus
{
    public class ReceivedEventConsumedMessage
    {
        public string? Type { get; set; }
        public object? Event { get; set; }
        public TodoTaskDto? Entity { get; set; }

        public TEvent? GetEvent<TEvent>() where TEvent : IEvent =>
            JsonConvert.DeserializeObject<TEvent>(JsonConvert.SerializeObject(Event));
    }
}
