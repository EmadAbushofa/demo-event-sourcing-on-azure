using Todo.Query.Infrastructure.Abstractions.MessageObjects;

namespace Todo.Query.Abstractions
{
    public interface IMessagePublisher
    {
        void Send(EventConsumedMessage message);
    }
}