using Todo.ApiGateway.Models.TodoTasks;

namespace Todo.ApiGateway.Test.Live.Helpers
{
    public static class AssertEquality
    {
        public static void Of(CreateTaskInput input, TodoTaskOutput output)
        {
            Assert.NotNull(input);
            Assert.NotNull(output);

            Assert.Equal(input.Title, output.Title);
            Assert.Equal(input.DueDate.Date, output.DueDate);
            Assert.Equal(input.Note, output.Note);
        }

        public static void Of(UpdateInfoTaskInput input, TodoTaskOutput output)
        {
            Assert.NotNull(input);
            Assert.NotNull(output);

            Assert.Equal(input.Title, output.Title);
            Assert.Equal(input.Note, output.Note);
        }

        public static void Of(ChangeDueDateTaskInput input, TodoTaskOutput output)
        {
            Assert.NotNull(input);
            Assert.NotNull(output);

            Assert.Equal(input.DueDate.Date, output.DueDate);
        }
    }
}
