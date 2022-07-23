using Todo.Query.EventHandlers;

namespace Todo.Query.Abstractions
{
    public interface IMessagePublisher
    {
        void Send(EventConsumed notification);
    }
}