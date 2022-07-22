using Todo.Query.EventHandlers;

namespace Todo.Query.Infrastructure.Abstractions.MessageObjects
{
    public class EventConsumedMessage
    {
        public EventConsumedMessage(IEvent @event, TodoTaskDto entity)
        {
            Type = @event.GetType().Name;
            Event = @event;
            Entity = entity;
        }

        public string Type { get; private set; }
        public IEvent Event { get; private set; }
        public TodoTaskDto Entity { get; private set; }
    }
}
