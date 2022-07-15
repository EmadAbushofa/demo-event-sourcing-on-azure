using Todo.Command.Events;

namespace Todo.Command.Test.Fakers.Completed
{
    public class TaskCompletedFaker : EventFaker<TaskCompleted, object>
    {
        public TaskCompletedFaker()
        {
            RuleFor(e => e.Sequence, 1);
        }

        public TaskCompletedFaker For(TaskCreated taskCreated)
        {
            RuleFor(e => e.AggregateId, taskCreated.AggregateId);
            RuleFor(e => e.UserId, taskCreated.UserId);
            RuleFor(e => e.Sequence, taskCreated.Sequence + 1);
            RuleFor(e => e.DateTime, taskCreated.DateTime.AddMinutes(1));
            return this;
        }
    }
}
