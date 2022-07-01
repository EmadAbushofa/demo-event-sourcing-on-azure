using Todo.Command.Events;
using Todo.Command.Events.DataTypes;

namespace Todo.Command.Test.Fakers.TaskCreated
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
