using Todo.Query.EventHandlers.Created;

namespace Todo.Query.Test.Fakers.Created
{
    public class TaskCreatedFaker : EventFaker<TaskCreated, TaskCreatedData>
    {
        public TaskCreatedFaker()
        {
            RuleFor(e => e.Sequence, 1);
            RuleFor(e => e.Data, new TaskCreatedDataFaker());
        }

        public TaskCreatedFaker RuleForTitle(string title)
        {
            var dataFaker = new TaskCreatedDataFaker()
                .RuleFor(e => e.Title, title);

            RuleFor(e => e.Data, dataFaker);

            return this;
        }

        public (TaskCreated, TaskCreated) Generate2EventsWithSameTitle(bool sameUser = true)
        {
            var @event = Generate();

            var secondEventFaker = new TaskCreatedFaker()
                .RuleForTitle(@event.Data.Title);

            var secondEvent = sameUser
                ? secondEventFaker.RuleFor(e => e.UserId, @event.UserId).Generate()
                : secondEventFaker.Generate();

            return (@event, secondEvent);
        }

        public (TaskCreated, TaskCreated) Generate2EventsForSameUser()
        {
            var @event = Generate();

            var secondEvent = new TaskCreatedFaker().RuleFor(e => e.UserId, @event.UserId).Generate();

            return (@event, secondEvent);
        }
    }
}
