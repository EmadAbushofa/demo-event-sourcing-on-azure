using Todo.Query.EventHandlers.InfoUpdated;

namespace Todo.Query.Test.Fakers.InfoUpdated
{
    public class TaskInfoUpdatedFaker : EventFaker<TaskInfoUpdated, TaskInfoUpdatedData>
    {
        public TaskInfoUpdatedFaker()
        {
            RuleFor(e => e.Sequence, 1);
            RuleFor(e => e.Data, new TaskInfoUpdatedDataFaker());
        }
    }
}
