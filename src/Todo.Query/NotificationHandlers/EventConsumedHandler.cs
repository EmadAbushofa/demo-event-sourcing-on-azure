using MediatR;
using Todo.Query.Abstractions;
using Todo.Query.EventHandlers;

namespace Todo.Query.NotificationHandlers
{
    public class EventConsumedHandler : INotificationHandler<EventConsumed>
    {
        private readonly IMessagePublisher _publisher;

        public EventConsumedHandler(IMessagePublisher publisher)
        {
            _publisher = publisher;
        }

        public Task Handle(EventConsumed notification, CancellationToken cancellationToken)
        {
            _publisher.Send(notification);
            return Task.CompletedTask;
        }
    }
}
