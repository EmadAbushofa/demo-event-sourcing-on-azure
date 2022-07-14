using Todo.Query.EventHandlers.DueDateChanged;

namespace Todo.Query.Test.Fakers.DueDateChanged
{
    public class TaskDueDateChangedDataFaker : RecordFaker<TaskDueDateChangedData>
    {
        public TaskDueDateChangedDataFaker()
        {
            RuleFor(e => e.DueDate, faker => DateOnly.FromDateTime(faker.Date.Future()));
        }
    }
}
