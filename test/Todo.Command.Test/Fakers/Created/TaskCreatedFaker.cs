using Todo.Command.Events;
using Todo.Command.Events.DataTypes;

namespace Todo.Command.Test.Fakers.Created
{
    public class TaskCreatedFaker : EventFaker<TaskCreated, TaskCreatedData>
    {
        public TaskCreatedFaker()
        {
            RuleFor(e => e.Sequence, 1);
            RuleFor(e => e.Data, new TaskCreatedDataFaker());
        }
    }
}
