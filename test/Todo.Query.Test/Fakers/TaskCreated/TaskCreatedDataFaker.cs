using Todo.Query.Features.Create;

namespace Todo.Query.Test.Fakers.TaskCreated
{
    public class TaskCreatedDataFaker : RecordFaker<TaskCreatedData>
    {
        public TaskCreatedDataFaker()
        {
            RuleFor(e => e.Title, faker => faker.Lorem.Sentence());
            RuleFor(e => e.DueDate, faker => faker.Date.Future());
            RuleFor(e => e.Note, faker => faker.Lorem.Paragraph());
        }
    }
}
