using Todo.Command.Events.DataTypes;

namespace Todo.Command.Test.Fakers.Created
{
    public class TaskCreatedDataFaker : RecordFaker<TaskCreatedData>
    {
        public TaskCreatedDataFaker()
        {
            RuleFor(e => e.Title, faker => faker.Lorem.Word());
            RuleFor(e => e.DueDate, faker => faker.Date.Future().Date);
            RuleFor(e => e.Note, faker => faker.Lorem.Sentence());
        }
    }
}
