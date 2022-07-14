using Todo.Query.EventHandlers.Created;

namespace Todo.Query.Test.Fakers.Created
{
    public class TaskCreatedDataFaker : RecordFaker<TaskCreatedData>
    {
        public TaskCreatedDataFaker()
        {
            RuleFor(e => e.Title, faker => faker.Lorem.Word());
            RuleFor(e => e.DueDate, faker => faker.Date.FutureDateOnly());
            RuleFor(e => e.Note, faker => faker.Lorem.Paragraph());
        }
    }
}
