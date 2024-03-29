﻿using Todo.Query.EventHandlers.DueDateChanged;

namespace Todo.Query.Test.Fakers.DueDateChanged
{
    public class TaskDueDateChangedDataFaker : RecordFaker<TaskDueDateChangedData>
    {
        public TaskDueDateChangedDataFaker()
        {
            RuleFor(e => e.DueDate, faker => faker.Date.Future().Date);
        }
    }
}
