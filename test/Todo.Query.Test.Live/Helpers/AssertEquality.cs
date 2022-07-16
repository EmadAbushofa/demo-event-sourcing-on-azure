using Todo.Query.Entities;
using Todo.Query.EventHandlers;
using Todo.Query.Infrastructure.Abstractions.MessageObjects;
using Todo.Query.Test.Live.EventBus;

namespace Todo.Query.Test.Live.Helpers
{
    public static class AssertEquality
    {
        public static void OfEventAndEntityAndMessage<TEvent>(IEvent @event, TodoTask? todoTask, ReceivedEventConsumedMessage? message)
            where TEvent : IEvent
        {
            Assert.NotNull(message);
            OfBothEvents(@event, message!.GetEvent<TEvent>());
            OfTaskAndDto(todoTask, message.Entity);
        }

        private static void OfBothEvents(IEvent @event1, IEvent? @event2)
        {
            Assert.NotNull(event1);
            Assert.NotNull(event2);

            Assert.Equal(event1.AggregateId, event2!.AggregateId);
            Assert.Equal(event1.UserId, event2.UserId);
            Assert.Equal(event1.Sequence, @event2.Sequence);
            Assert.Equal(event1.Version, event2.Version);
            Assert.Equal(@event1.DateTime, @event2.DateTime, TimeSpan.FromMinutes(1));
        }

        private static void OfTaskAndDto(TodoTask? todoTask, TodoTaskDto? dto)
        {
            Assert.NotNull(todoTask);
            Assert.NotNull(dto);

            Assert.Equal(todoTask!.Title, dto!.Title);
            Assert.Equal(todoTask.IsUniqueTitle, dto.IsUniqueTitle);
            Assert.Equal(todoTask.IsCompleted, dto.IsCompleted);
            Assert.Equal(todoTask.UserId, dto.UserId);
            Assert.Equal(todoTask.Note, dto.Note);
            Assert.Equal(todoTask.CreatedAt, dto.CreatedAt, TimeSpan.FromMinutes(1));
            Assert.Equal(todoTask.LastUpdate, dto.LastUpdate, TimeSpan.FromMinutes(1));
            Assert.Equal(todoTask.DueDate, dto.DueDate, TimeSpan.FromMinutes(1));
        }
    }
}