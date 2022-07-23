using Grpc.Core;
using Todo.Query.Abstractions;
using Todo.Query.EventHandlers;
using Todo.Query.Extensions;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.Services
{
    public class NotificationsStreamService : IMessagePublisher
    {
        private readonly List<IAsyncStreamWriter<NotificationResponse>> _streams = new();

        public void AddStream(IAsyncStreamWriter<NotificationResponse> streamWriter) => _streams.Add(streamWriter);
        public void RemoveStream(IAsyncStreamWriter<NotificationResponse> streamWriter) => _streams.Remove(streamWriter);

        public void Send(EventConsumed notification)
        {
            var response = new NotificationResponse()
            {
                Event = new ConsumedEvent()
                {
                    AggregateId = notification.Event.AggregateId.ToString(),
                    DateTime = notification.Event.DateTime.ToUtcTimestamp(),
                    Sequence = notification.Event.Sequence,
                    UserId = notification.Event.UserId,
                    Version = notification.Event.Version,
                    Data = notification.Event.GetDataAsJson(),
                },
                Task = new TaskOutput()
                {
                    Id = notification.TodoTask.Id.ToString(),
                    DueDate = notification.TodoTask.DueDate.ToUtcTimestamp(),
                    IsCompleted = notification.TodoTask.IsCompleted,
                    Note = notification.TodoTask.Note,
                    Title = notification.TodoTask.Title,
                    UserId = notification.TodoTask.UserId,
                    DuplicateTitle = !notification.TodoTask.IsUniqueTitle,
                },
                Type = notification.Event.GetType().Name,
            };

            _streams.ForEach(stream => stream.WriteAsync(response));
        }
    }
}
