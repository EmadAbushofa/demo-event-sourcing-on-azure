using MediatR;
using Todo.Query.Entities;

namespace Todo.Query.EventHandlers
{
    public record EventConsumed(
        IEvent Event,
        TodoTask TodoTask
    ) : INotification;
}
