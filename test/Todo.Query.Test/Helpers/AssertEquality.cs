using Todo.Query.Entities;
using Todo.Query.EventHandlers.Created;
using Todo.Query.Test.Client.TodoProto;

namespace Todo.Query.Test.Helpers
{
    public static class AssertEquality
    {
        public static void OfEventAndTask(TaskCreatedEvent @event, TodoTask? todoTask)
        {
            Assert.NotNull(todoTask);

            if (todoTask == null) throw new ArgumentNullException(nameof(todoTask));

            Assert.Equal(@event.AggregateId, todoTask.Id);
            Assert.Equal(@event.UserId, todoTask.UserId);
            Assert.Equal(@event.DateTime, todoTask.CreatedAt, TimeSpan.FromMinutes(1));
            Assert.Equal(@event.DateTime, todoTask.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.False(todoTask.IsCompleted);
            Assert.Equal(@event.Data.Title, todoTask.Title);
            Assert.Equal(@event.Data.Title, todoTask.ActualTitle);
            Assert.Equal(@event.Data.Title.ToUpper(), todoTask.NormalizedTitle);
            Assert.Equal(@event.Data.DueDate, todoTask.DueDate);
            Assert.Equal(@event.Data.Note, todoTask.Note);
            Assert.True(todoTask.IsUniqueTitle);
        }

        public static void OfTaskAndResponse(TodoTask todoTask, FindResponse response)
        {
            Assert.NotNull(response);

            Assert.Equal(todoTask.Id.ToString(), response.Id);
            Assert.Equal(todoTask.UserId, response.UserId);
            Assert.Equal(todoTask.IsCompleted, response.IsCompleted);
            Assert.Equal(todoTask.Title, response.Title);
            Assert.Equal(todoTask.DueDate, response.DueDate.ToDateTime());
            Assert.Equal(todoTask.Note, response.Note);
        }
    }
}