using Newtonsoft.Json;
using Todo.Query.Entities;
using Todo.Query.EventHandlers;
using Todo.Query.EventHandlers.Completed;
using Todo.Query.EventHandlers.Created;
using Todo.Query.EventHandlers.DueDateChanged;
using Todo.Query.EventHandlers.InfoUpdated;
using Todo.Query.EventHandlers.Uncompleted;
using Todo.Query.Test.Client.TodoProto;

namespace Todo.Query.Test.Helpers
{
    public static class AssertEquality
    {
        public static void OfEventAndTask(TaskCreated @event, TodoTask? todoTask, bool isUnique = true)
        {
            Assert.NotNull(todoTask);

            Assert.Equal(@event.AggregateId, todoTask!.Id);
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

            Assert.Equal(@event.AggregateId, todoTask!.Id);
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

            Assert.Equal(@event.AggregateId, todoTask!.Id);
            Assert.Equal(@event.Sequence, todoTask.Sequence);
            Assert.Equal(@event.UserId, todoTask.UserId);
            Assert.Equal(@event.DateTime, todoTask.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.Equal(@event.Data.DueDate, todoTask.DueDate);
        }

        public static void OfEventAndTask(TaskCompleted @event, TodoTask? todoTask)
        {
            Assert.NotNull(todoTask);

            Assert.Equal(@event.AggregateId, todoTask!.Id);
            Assert.Equal(@event.Sequence, todoTask.Sequence);
            Assert.Equal(@event.UserId, todoTask.UserId);
            Assert.Equal(@event.DateTime, todoTask.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.True(todoTask.IsCompleted);
        }

        public static void OfEventAndTask(TaskUncompleted @event, TodoTask? todoTask)
        {
            Assert.NotNull(todoTask);

            Assert.Equal(@event.AggregateId, todoTask!.Id);
            Assert.Equal(@event.Sequence, todoTask.Sequence);
            Assert.Equal(@event.UserId, todoTask.UserId);
            Assert.Equal(@event.DateTime, todoTask.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.False(todoTask.IsCompleted);
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

        public static void OfExpectationAndFilterResponse(
            int expectedPage,
            int expectedSize,
            int expectedTotal,
            int expectedTasks,
            FilterResponse response
        )
        {
            Assert.Equal(expectedPage, response.Page);
            Assert.Equal(expectedSize, response.Size);
            Assert.Equal(expectedTotal, response.Total);
            Assert.Equal(expectedTasks, response.Tasks.Count);
        }

        public static void OfTasksAndFilterOutputs(List<TodoTask> todoTasks, FilterResponse response)
        {
            Assert.All(todoTasks, task =>
            {
                var output = response.Tasks.SingleOrDefault(t => t.Id == task.Id.ToString());
                OfTaskAndFilterOutput(task, output);
            });
        }

        public static void OfTaskAndFilterOutput(TodoTask todoTask, TaskFilterOutput? output)
        {
            Assert.NotNull(output);

            Assert.Equal(todoTask.Id.ToString(), output!.Id);
            Assert.Equal(todoTask.UserId, output.UserId);
            Assert.Equal(todoTask.IsCompleted, output.IsCompleted);
            Assert.Equal(todoTask.Title, output.Title);
            Assert.Equal(todoTask.DueDate, output.DueDate.ToDateTime());
        }

        public static void OfEventAndEntityAndNotification(
            IEvent @event,
            TodoTask? todoTask,
            NotificationResponse response
        )
        {
            Assert.NotNull(todoTask);
            Assert.Equal(@event.GetType().Name, response.Type);
            OfBothEvents(@event, response.Event);
            OfTaskAndDto(todoTask!, response.Task);
        }

        private static void OfBothEvents(IEvent @event, ConsumedEvent consumedEvent)
        {
            Assert.Equal(@event.AggregateId.ToString(), consumedEvent.AggregateId);
            Assert.Equal(@event.UserId, consumedEvent.UserId);
            Assert.Equal(@event.Sequence, consumedEvent.Sequence);
            Assert.Equal(@event.Version, consumedEvent.Version);
            Assert.Equal(@event.DateTime, consumedEvent.DateTime.ToDateTime(), TimeSpan.FromMinutes(1));
            Assert.NotEmpty(consumedEvent.Data);
        }

        private static void OfTaskAndDto(TodoTask todoTask, TaskOutput dto)
        {
            Assert.Equal(todoTask!.Title, dto!.Title);
            Assert.Equal(todoTask.IsCompleted, dto.IsCompleted);
            Assert.Equal(todoTask.UserId, dto.UserId);
            Assert.Equal(todoTask.Note, dto.Note);
            Assert.Equal(todoTask.DueDate, dto.DueDate.ToDateTime().Date);
        }
    }
}