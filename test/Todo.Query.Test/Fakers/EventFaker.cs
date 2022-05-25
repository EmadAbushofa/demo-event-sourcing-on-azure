using Todo.Query.Features;

namespace Todo.Query.Test.Fakers
{
    public abstract class EventFaker<TEvent, TData> : RecordFaker<TEvent>
        where TEvent : Event<TData>
    {
        protected EventFaker()
        {
            RuleFor(e => e.AggregateId, faker => faker.Random.Guid());
            RuleFor(e => e.Sequence, faker => faker.Random.Int());
            RuleFor(e => e.UserId, faker => faker.Random.Guid().ToString());
            RuleFor(e => e.DateTime, faker => faker.Date.Recent());
            RuleFor(e => e.Version, 1);
        }
    }
}
