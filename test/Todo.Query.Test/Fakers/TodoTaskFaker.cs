using Todo.Query.Entities;

namespace Todo.Query.Test.Fakers
{
    public class TodoTaskFaker : RecordFaker<TodoTask>
    {
        public TodoTaskFaker()
        {
            RuleFor(e => e.UserId, f => f.Random.Guid().ToString());
            RuleFor(e => e.DueDate, f => f.Date.Soon().ToUniversalTime().Date);
            RuleFor(e => e.IsCompleted, f => f.Random.Bool());
            RuleFor(e => e.ActualTitle, f => f.Lorem.Word());
            RuleFor(e => e.Title, (_, t) => t.ActualTitle);
            RuleFor(e => e.NormalizedTitle, (_, t) => t.Title.ToUpper());
            RuleFor(e => e.IsUniqueTitle, true);
            RuleFor(e => e.Note, f => f.Lorem.Sentences());
            RuleFor(e => e.CreatedAt, f => f.Date.Recent(days: 3).ToUniversalTime());
            RuleFor(e => e.LastUpdate, f => f.Date.Recent().ToUniversalTime());
        }
    }
}
