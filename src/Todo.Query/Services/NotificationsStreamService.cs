using Grpc.Core;
using Todo.Query.Abstractions;
using Todo.Query.Extensions;
using Todo.Query.Infrastructure.Abstractions.MessageObjects;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.Services
{
    public class NotificationsStreamService : IMessagePublisher
    {
        private readonly List<IAsyncStreamWriter<NotificationResponse>> _streams = new();

        public void AddStream(IAsyncStreamWriter<NotificationResponse> streamWriter) => _streams.Add(streamWriter);
        public void RemoveStream(IAsyncStreamWriter<NotificationResponse> streamWriter) => _streams.Remove(streamWriter);

        public void Send(EventConsumedMessage message)
        {
            var response = new NotificationResponse()
            {
                Event = new ConsumedEvent()
                {
                    AggregateId = message.Event.AggregateId.ToString(),
                    DateTime = message.Event.DateTime.ToUtcTimestamp(),
                    Sequence = message.Event.Sequence,
                    UserId = message.Event.UserId,
                    Version = message.Event.Version,
                    Data = message.Event.GetDataAsJson(),
                },
                Task = new TaskOutput()
                {
                    Id = message.Entity.Id.ToString(),
                    DueDate = message.Entity.DueDate.ToUtcTimestamp(),
                    IsCompleted = message.Entity.IsCompleted,
                    Note = message.Entity.Note,
                    Title = message.Entity.Title,
                    UserId = message.Entity.UserId,
                },
                Type = message.Type,
            };

            _streams.ForEach(stream => stream.WriteAsync(response));
        }
    }
}
