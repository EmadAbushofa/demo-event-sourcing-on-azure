﻿using Todo.Command.Events;
using Todo.Command.Extensions;
using Todo.Command.Test.Client.TodoProto;

namespace Todo.Command.Test.Helpers
{
    public static class AssertEquality
    {
        public static void OfCreatedEvent(Event @event, CreateRequest request, Response response)
        {
            var created = (TaskCreated)@event;

            Assert.Equal(response.Id, created.AggregateId.ToString());
            Assert.Equal(request.UserId, created.UserId);
            Assert.Equal(DateTime.UtcNow, created.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(1, created.Sequence);
            Assert.Equal(1, created.Version);
            Assert.Equal(request.Title, created.Data.Title);
            Assert.Equal(request.DueDate.ToDate(), created.Data.DueDate);
            Assert.Equal(request.Note, created.Data.Note);
        }

        public static void OfInfoUpdatedEvent(Event @event, UpdateInfoRequest request, Response response, int expectedSequence)
        {
            var infoUpdated = (TaskInfoUpdated)@event;

            Assert.Equal(response.Id, infoUpdated.AggregateId.ToString());
            Assert.Equal(request.UserId, infoUpdated.UserId);
            Assert.Equal(DateTime.UtcNow, infoUpdated.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(expectedSequence, infoUpdated.Sequence);
            Assert.Equal(1, infoUpdated.Version);
            Assert.Equal(request.Title, infoUpdated.Data.Title);
            Assert.Equal(request.Note, infoUpdated.Data.Note);
        }

        public static void OfDueDateChangedEvent(Event @event, ChangeDueDateRequest request, Response response, int expectedSequence)
        {
            var dueDateChanged = (TaskDueDateChanged)@event;

            Assert.Equal(response.Id, dueDateChanged.AggregateId.ToString());
            Assert.Equal(request.UserId, dueDateChanged.UserId);
            Assert.Equal(DateTime.UtcNow, dueDateChanged.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(expectedSequence, dueDateChanged.Sequence);
            Assert.Equal(1, dueDateChanged.Version);
            Assert.Equal(request.DueDate.ToDate(), dueDateChanged.Data.DueDate);
        }

        public static void OfCompletedEvent(Event @event, CompleteRequest request, Response response, int expectedSequence)
        {
            var taskCompleted = (TaskCompleted)@event;

            Assert.Equal(response.Id, taskCompleted.AggregateId.ToString());
            Assert.Equal(request.UserId, taskCompleted.UserId);
            Assert.Equal(DateTime.UtcNow, taskCompleted.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(expectedSequence, taskCompleted.Sequence);
            Assert.Equal(1, taskCompleted.Version);
        }

        public static void OfUncompletedEvent(Event @event, CompleteRequest request, Response response, int expectedSequence)
        {
            var taskUncompleted = (TaskUncompleted)@event;

            Assert.Equal(response.Id, taskUncompleted.AggregateId.ToString());
            Assert.Equal(request.UserId, taskUncompleted.UserId);
            Assert.Equal(DateTime.UtcNow, taskUncompleted.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(expectedSequence, taskUncompleted.Sequence);
            Assert.Equal(1, taskUncompleted.Version);
        }

        public static void OfDeletedEvent(Event @event, DeleteRequest request, Response response, int expectedSequence)
        {
            var taskDeleted = (TaskDeleted)@event;

            Assert.Equal(response.Id, taskDeleted.AggregateId.ToString());
            Assert.Equal(request.UserId, taskDeleted.UserId);
            Assert.Equal(DateTime.UtcNow, taskDeleted.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(expectedSequence, taskDeleted.Sequence);
            Assert.Equal(1, taskDeleted.Version);
        }
    }
}