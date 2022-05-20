using Newtonsoft.Json;
using Todo.Command.Events;

namespace Todo.Command.Persistence
{
    public class Document
    {
        public Document()
        {

        }

        public Document(Event @event)
        {
            Id = @event.Sequence.ToString();
            AggregateId = @event.AggregateId.ToString();
            Sequence = @event.Sequence;
            UserId = @event.UserId;
            DateTime = @event.DateTime;
            Type = @event.Type.ToString();
            Version = @event.Version;
            Data = ((dynamic)@event).Data;
        }

        public string? Id { get; set; }
        public string? AggregateId { get; set; }
        public int Sequence { get; set; }
        public string? UserId { get; set; }
        public DateTime DateTime { get; set; }
        public string? Type { get; set; }
        public object? Data { get; set; }
        public int Version { get; set; }

        public Event ToEvent()
        {
            var json = JsonConvert.SerializeObject(this);

            return Type switch
            {
                nameof(EventType.TaskCreated) => Deserialize(json),
                _ => throw new InvalidCastException("Unkown event type " + Type),
            };
        }

        private static Event Deserialize(string json) => JsonConvert.DeserializeObject<TaskCreatedEvent>(json);
    }
}
