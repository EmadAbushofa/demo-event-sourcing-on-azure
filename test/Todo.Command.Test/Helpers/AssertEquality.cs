﻿using Todo.Command.Events;
using Todo.Command.TodoProto;

namespace Todo.Command.Test.Helpers
{
    public static class AssertEquality
    {
        public static void OfCreatedEvent(Event @event, CreateRequest request, Response response)
        {
            var createdEvent = (TaskCreatedEvent)@event;

            Assert.Equal(response.Id, createdEvent.AggregateId.ToString());
            Assert.Equal(request.UserId, createdEvent.UserId);
            Assert.Equal(EventType.TaskCreated, createdEvent.Type);
            Assert.Equal(DateTime.UtcNow, createdEvent.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(1, createdEvent.Sequence);
            Assert.Equal(1, createdEvent.Version);
        }
    }
}