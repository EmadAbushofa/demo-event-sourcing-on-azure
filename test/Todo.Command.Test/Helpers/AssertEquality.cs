using Todo.Command.Events;
using Todo.Command.Test.Client.TodoProto;

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
            Assert.Equal(request.Title, createdEvent.Data.Title);
            Assert.Equal(request.DueDate.ToDateTime().Date, createdEvent.Data.DueDate);
            Assert.Equal(request.Note, createdEvent.Data.Note);
        }

        public static void OfInfoUpdatedEvent(Event @event, UpdateInfoRequest request, Response response, int expectedSequence)
        {
            var infoUpdatedEvent = (TaskInfoUpdatedEvent)@event;

            Assert.Equal(response.Id, infoUpdatedEvent.AggregateId.ToString());
            Assert.Equal(request.UserId, infoUpdatedEvent.UserId);
            Assert.Equal(EventType.TaskInfoUpdated, infoUpdatedEvent.Type);
            Assert.Equal(DateTime.UtcNow, infoUpdatedEvent.DateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(expectedSequence, infoUpdatedEvent.Sequence);
            Assert.Equal(1, infoUpdatedEvent.Version);
            Assert.Equal(request.Title, infoUpdatedEvent.Data.Title);
            Assert.Equal(request.Note, infoUpdatedEvent.Data.Note);
        }
    }
}