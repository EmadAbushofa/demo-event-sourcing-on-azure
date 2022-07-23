using Todo.Query.Entities;

namespace Todo.Query.Test.Fakers
{
    public class TodoTaskFaker : RecordFaker<TodoTask>
    {
        public TodoTaskFaker()
        {
            RuleFor(e => e.UserId, f => f.Random.Guid().ToString());
            RuleFor(e => e.Sequence, f => f.Random.Int(3, 9));
            RuleFor(e => e.DueDate, f => f.Date.Soon().Date);
            RuleFor(e => e.IsCompleted, f => f.Random.Bool());
            RuleFor(e => e.Title, f => f.Lorem.Word());
            RuleFor(e => e.NormalizedTitle, (_, t) => t.Title.ToUpper());
            RuleFor(e => e.IsUniqueTitle, true);
            RuleFor(e => e.Note, f => f.Lorem.Sentences());
            RuleFor(e => e.CreatedAt, f => f.Date.Recent(days: 3).ToUniversalTime());
            RuleFor(e => e.LastUpdate, f => f.Date.Recent().ToUniversalTime());
        }

        public TodoTask GeneratWithRandomUniqueTitle(bool? isCompleted = null)
        {
            if (isCompleted != null)
                RuleFor(e => e.IsCompleted, isCompleted.Value);

            return RuleFor(e => e.IsUniqueTitle, f => f.Random.Bool()).Generate();
        }

        public List<TodoTask> GeneratWithRandomUniqueTitle(int count) =>
            RuleFor(e => e.IsUniqueTitle, f => f.Random.Bool()).Generate(count);

        public static TodoTask GenerateCompletedTask(bool isCompleted = true)
        {
            return new TodoTaskFaker()
                .RuleFor(f => f.IsCompleted, isCompleted)
                .Generate();
        }

        public static TodoTask WithSameUser(TodoTask todoTask, bool isCompleted)
        {
            return new TodoTaskFaker()
                .RuleFor(f => f.UserId, todoTask.UserId)
                .RuleFor(f => f.IsCompleted, isCompleted)
                .Generate();
        }
    }
}
