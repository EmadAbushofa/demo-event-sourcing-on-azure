using MediatR;
using Todo.Query.Abstractions;
using Todo.Query.EventHandlers;
using Todo.Query.Infrastructure.Abstractions.MessageObjects;

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
            var message = new EventConsumedMessage(
                notification.Event,
                new TodoTaskDto(notification.TodoTask)
            );
            _publisher.Send(message);
            return Task.CompletedTask;
        }
    }
}
