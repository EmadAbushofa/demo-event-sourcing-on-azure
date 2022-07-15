using Todo.Query.Entities;
using Todo.Query.EventHandlers.Uncompleted;

namespace Todo.Query.Test.Fakers.Uncompleted
{
    public class TaskUncompletedFaker : EventFaker<TaskUncompleted, object>
    {
        public TaskUncompletedFaker()
        {
            RuleFor(e => e.Sequence, f => f.Random.Int(1, 9));
        }

        public TaskUncompletedFaker For(TodoTask todoTask, int? sequence = null)
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
