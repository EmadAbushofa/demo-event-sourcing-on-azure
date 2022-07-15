using Todo.Command.Events;

namespace Todo.Command.Test.Fakers.Deleted
{
    public class TaskDeletedFaker : EventFaker<TaskDeleted, object>
    {
        public TaskDeletedFaker()
        {
            RuleFor(e => e.Sequence, 1);
        }

        public TaskDeletedFaker For(TaskCreated taskCreated)
        {
            RuleFor(e => e.AggregateId, taskCreated.AggregateId);
            RuleFor(e => e.UserId, taskCreated.UserId);
            RuleFor(e => e.Sequence, taskCreated.Sequence + 1);
            RuleFor(e => e.DateTime, taskCreated.DateTime.AddMinutes(1));
            return this;
        }
    }
}
