using Todo.Command.Events;

namespace Todo.Command.Test.Fakers.Uncompleted
{
    public class TaskUncompletedFaker : EventFaker<TaskUncompleted, object>
    {
        public TaskUncompletedFaker()
        {
            RuleFor(e => e.Sequence, 1);
        }

        public TaskUncompletedFaker For(TaskCompleted taskCompleted)
        {
            RuleFor(e => e.AggregateId, taskCompleted.AggregateId);
            RuleFor(e => e.UserId, taskCompleted.UserId);
            RuleFor(e => e.Sequence, taskCompleted.Sequence + 1);
            RuleFor(e => e.DateTime, taskCompleted.DateTime.AddMinutes(1));
            return this;
        }
    }
}
