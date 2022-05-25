using Todo.Query.Features.Create;

namespace Todo.Query.Test.Fakers.TaskCreated
{
    public class TaskCreatedEventFaker : EventFaker<TaskCreatedEvent, TaskCreatedData>
    {
        public TaskCreatedEventFaker()
        {
            RuleFor(e => e.Sequence, 1);
            RuleFor(e => e.Data, new TaskCreatedDataFaker());
        }
    }
}
