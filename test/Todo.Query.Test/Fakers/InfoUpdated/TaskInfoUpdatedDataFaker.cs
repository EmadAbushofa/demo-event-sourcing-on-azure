using Todo.Query.EventHandlers.InfoUpdated;

namespace Todo.Query.Test.Fakers.InfoUpdated
{
    public class TaskInfoUpdatedDataFaker : RecordFaker<TaskInfoUpdatedData>
    {
        public TaskInfoUpdatedDataFaker()
        {
            RuleFor(e => e.Title, faker => faker.Lorem.Sentence());
            RuleFor(e => e.Note, faker => faker.Lorem.Paragraph());
        }
    }
}
