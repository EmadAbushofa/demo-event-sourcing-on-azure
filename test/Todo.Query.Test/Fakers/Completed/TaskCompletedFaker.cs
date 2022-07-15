using Todo.Query.Entities;
using Todo.Query.EventHandlers.Completed;

namespace Todo.Query.Test.Fakers.Completed
{
    public class TaskCompletedFaker : EventFaker<TaskCompleted, object>
    {
        public TaskCompletedFaker()
        {
            RuleFor(e => e.Sequence, f => f.Random.Int(1, 9));
        }

        public TaskCompletedFaker For(TodoTask todoTask, int? sequence = null)
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
