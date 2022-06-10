﻿using Todo.Query.Abstractions;
using Todo.Query.Features.Create;

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
            Assert.Equal(@event.DateTime, todoTask.CreatedAt);
            Assert.Equal(@event.DateTime, todoTask.LastUpdate);
            Assert.False(todoTask.IsCompleted);
            Assert.Equal(@event.Data.Title, todoTask.Title);
            Assert.Equal(@event.Data.DueDate, todoTask.DueDate);
            Assert.Equal(@event.Data.Note, todoTask.Note);
        }
    }
}