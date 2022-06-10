using System;

namespace Todo.EventPublisher.Entities
{
    public class Event
    {
        public string Id { get; set; }
        public string AggregateId { get; set; }
        public int Sequence { get; set; }
        public string UserId { get; set; }
        public DateTime DateTime { get; set; }
        public string Type { get; set; }
        public object Data { get; set; }
        public int Version { get; set; }
    }
}
