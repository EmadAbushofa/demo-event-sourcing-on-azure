using Newtonsoft.Json;
using Todo.Command.Events;

namespace Todo.Command.Infrastructure.Persistence
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
            Type = @event.Type;
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
            if (Type == null)
                throw new InvalidOperationException("Type cannot be null");

            var json = JsonConvert.SerializeObject(this);

            return Deserialize(json, Type);
        }

        private static Event Deserialize(string json, string type)
            => (Event)JsonConvert.DeserializeObject(json, System.Type.GetType(GetTypeName(type)));

        private static string GetTypeName(string type)
            => typeof(TaskCreated).FullName?.Replace(nameof(TaskCreated), type)
                ?? throw new TypeLoadException("Cannot get full name of event.");
    }
}
