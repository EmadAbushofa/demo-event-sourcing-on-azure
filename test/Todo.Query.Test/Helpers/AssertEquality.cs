using Newtonsoft.Json;
using Todo.Query.Entities;
using Todo.Query.EventHandlers.Created;
using Todo.Query.EventHandlers.DueDateChanged;
using Todo.Query.EventHandlers.InfoUpdated;
using Todo.Query.Test.Client.TodoProto;

namespace Todo.Query.Test.Helpers
{
    public static class AssertEquality
    {
        public static void OfEventAndTask(TaskCreated @event, TodoTask? todoTask, bool isUnique = true)
        {
            Assert.NotNull(todoTask);

            if (todoTask == null) throw new ArgumentNullException(nameof(todoTask));

            Assert.Equal(@event.AggregateId, todoTask.Id);
            Assert.Equal(@event.Sequence, todoTask.Sequence);
            Assert.Equal(@event.UserId, todoTask.UserId);
            Assert.Equal(@event.DateTime, todoTask.CreatedAt, TimeSpan.FromMinutes(1));
            Assert.Equal(@event.DateTime, todoTask.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.False(todoTask.IsCompleted);
            Assert.Equal(@event.Data.Title, todoTask.Title);
            Assert.Equal(@event.Data.DueDate, todoTask.DueDate);
            Assert.Equal(@event.Data.Note, todoTask.Note);

            if (isUnique)
            {
                Assert.True(todoTask.IsUniqueTitle);
                Assert.Equal(@event.Data.Title.ToUpper(), todoTask.NormalizedTitle);
            }
            else
            {
                Assert.False(todoTask.IsUniqueTitle);
                Assert.StartsWith(@event.Data.Title.ToUpper() + "_COPY", todoTask.NormalizedTitle);
            }
        }

        public static void OfEventAndTask(TaskInfoUpdated @event, TodoTask? todoTask, bool isUnique = true)
        {
            Assert.NotNull(todoTask);

            if (todoTask == null) throw new ArgumentNullException(nameof(todoTask));

            Assert.Equal(@event.AggregateId, todoTask.Id);
            Assert.Equal(@event.Sequence, todoTask.Sequence);
            Assert.Equal(@event.UserId, todoTask.UserId);
            Assert.Equal(@event.DateTime, todoTask.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.Equal(@event.Data.Title, todoTask.Title);
            Assert.Equal(@event.Data.Note, todoTask.Note);

            if (isUnique)
            {
                Assert.True(todoTask.IsUniqueTitle);
                Assert.Equal(@event.Data.Title.ToUpper(), todoTask.NormalizedTitle);
            }
            else
            {
                Assert.False(todoTask.IsUniqueTitle);
                Assert.StartsWith(@event.Data.Title.ToUpper() + "_COPY", todoTask.NormalizedTitle);
            }
        }

        public static void OfEventAndTask(TaskDueDateChanged @event, TodoTask? todoTask)
        {
            Assert.NotNull(todoTask);

            if (todoTask == null) throw new ArgumentNullException(nameof(todoTask));

            Assert.Equal(@event.AggregateId, todoTask.Id);
            Assert.Equal(@event.Sequence, todoTask.Sequence);
            Assert.Equal(@event.UserId, todoTask.UserId);
            Assert.Equal(@event.DateTime, todoTask.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.Equal(@event.Data.DueDate, todoTask.DueDate);
        }

        public static void OfBoth(object? obj1, object? obj2)
        {
            Assert.NotNull(obj1);
            Assert.NotNull(obj2);
            var json1 = JsonConvert.SerializeObject(obj1);
            var json2 = JsonConvert.SerializeObject(obj2);
            Assert.Equal(json1, json2);
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