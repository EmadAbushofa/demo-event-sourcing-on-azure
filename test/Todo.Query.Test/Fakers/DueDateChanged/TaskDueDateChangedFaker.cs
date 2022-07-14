using Todo.Query.Entities;
using Todo.Query.EventHandlers.DueDateChanged;

namespace Todo.Query.Test.Fakers.DueDateChanged
{
    public class TaskDueDateChangedFaker : EventFaker<TaskDueDateChanged, TaskDueDateChangedData>
    {
        public TaskDueDateChangedFaker()
        {
            RuleFor(e => e.Sequence, f => f.Random.Int(1, 9));
            RuleFor(e => e.Data, new TaskDueDateChangedDataFaker());
        }

        public TaskDueDateChangedFaker RuleForDueDate(DateOnly date)
        {
            var dataFaker = new TaskDueDateChangedDataFaker()
                .RuleFor(e => e.DueDate, date);
            RuleFor(e => e.Data, dataFaker);
            return this;
        }

        public TaskDueDateChangedFaker For(TodoTask todoTask, int? sequence = null)
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
