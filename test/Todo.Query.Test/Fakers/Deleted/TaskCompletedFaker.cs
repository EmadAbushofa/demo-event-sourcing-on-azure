using Todo.Query.Entities;
using Todo.Query.EventHandlers.Deleted;

namespace Todo.Query.Test.Fakers.Deleted
{
    public class TaskDeletedFaker : EventFaker<TaskDeleted, object>
    {
        public TaskDeletedFaker()
        {
            RuleFor(e => e.Sequence, f => f.Random.Int(1, 9));
        }

        public TaskDeletedFaker For(TodoTask todoTask, int? sequence = null)
        {
            RuleFor(e => e.AggregateId, todoTask.Id);

            if (sequence != null)
                RuleFor(e => e.Sequence, sequence);
            else
                RuleFor(e => e.Sequence, todoTask.Sequence + 1);

            RuleFor(e => e.UserId, todoTask.UserId);
            return this;
        }
    }
}
