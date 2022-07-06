using Todo.Command.Events.DataTypes;

namespace Todo.Command.Test.Fakers.Created
{
    public class TaskCreatedDataFaker : RecordFaker<TaskCreatedData>
    {
        public TaskCreatedDataFaker()
        {
            RuleFor(e => e.Title, faker => faker.Lorem.Sentence());
            RuleFor(e => e.DueDate, faker => faker.Date.Future().ToUniversalTime().Date);
            RuleFor(e => e.Note, faker => faker.Lorem.Paragraph());
        }
    }
}
