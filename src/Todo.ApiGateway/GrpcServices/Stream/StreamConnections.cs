using Grpc.Core;
using Todo.ApiGateway.TodoProto.Channel;

namespace Todo.ApiGateway.GrpcServices.Stream
{
    public class StreamConnections
    {
        private readonly Dictionary<string, IAsyncStreamWriter<NotificationOutput>> _connections = new();

        public StreamConnections(string connectionId, IAsyncStreamWriter<NotificationOutput> streamWriter)
        {
            Add(connectionId, streamWriter);
        }

        public int Count => _connections.Count;

        public void Add(string connectionId, IAsyncStreamWriter<NotificationOutput> streamWriter) =>
            _connections.TryAdd(connectionId, streamWriter);

        public void Remove(string connectionId) =>
            _connections.Remove(connectionId);

        public Task WriteToAllConnectionsAsync(NotificationOutput output) =>
            Task.WhenAll(_connections.Select(c => c.Value.WriteAsync(output)));
    }
}
