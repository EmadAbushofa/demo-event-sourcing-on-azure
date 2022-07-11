using Todo.Query.Entities;
using Todo.Query.EventHandlers.InfoUpdated;

namespace Todo.Query.Test.Fakers.InfoUpdated
{
    public class TaskInfoUpdatedFaker : EventFaker<TaskInfoUpdated, TaskInfoUpdatedData>
    {
        public TaskInfoUpdatedFaker()
        {
            RuleFor(e => e.Sequence, f => f.Random.Int(1, 9));
            RuleFor(e => e.Data, new TaskInfoUpdatedDataFaker());
        }

        public TaskInfoUpdatedFaker RuleForTitle(string title)
        {
            var dataFaker = new TaskInfoUpdatedDataFaker()
                .RuleFor(e => e.Title, title);
            RuleFor(e => e.Data, dataFaker);
            return this;
        }

        public TaskInfoUpdatedFaker For(TodoTask todoTask, int? sequence = null)
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
