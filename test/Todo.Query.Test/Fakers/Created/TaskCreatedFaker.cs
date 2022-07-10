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


        public (TaskCreated, TaskCreated) Generate2EventsWithSameTitle(bool sameUser = true)
        {
            var @event = Generate();

            var dataFaker = new TaskCreatedDataFaker()
                .RuleFor(e => e.Title, @event.Data.Title);

            var secondEventFaker = new TaskCreatedFaker()
                .RuleFor(e => e.Data, dataFaker);

            var secondEvent = sameUser
                ? secondEventFaker.RuleFor(e => e.UserId, @event.UserId).Generate()
                : secondEventFaker.Generate();

            return (@event, secondEvent);
        }
    }
}
