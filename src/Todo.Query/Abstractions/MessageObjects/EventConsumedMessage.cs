using Todo.Query.EventHandlers;

namespace Todo.Query.Infrastructure.Abstractions.MessageObjects
{
    public class EventConsumedMessage
    {
        public EventConsumedMessage()
        {

        }

        public EventConsumedMessage(IEvent @event, TodoTaskDto entity)
        {
            Type = @event.GetType().Name;
            Event = @event;
            Entity = entity;
        }

        public string? Type { get; set; }
        public IEvent? Event { get; set; }
        public TodoTaskDto? Entity { get; set; }
    }
}
