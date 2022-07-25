using Grpc.Core;
using Todo.ApiGateway.TodoProto.Channel;

namespace Todo.ApiGateway.GrpcServices.Stream
{
    public class UserStreamConnections
    {
        private readonly Dictionary<string, StreamConnections> _userConnections = new();
        private readonly object _lockObject = new();

        public void Add(string userId, string connectionId, IAsyncStreamWriter<NotificationOutput> streamWriter)
        {
            lock (_lockObject)
            {
                if (!_userConnections.TryAdd(userId, new StreamConnections(connectionId, streamWriter)))
                {
                    _userConnections[userId].Add(connectionId, streamWriter);
                }
            }
        }

        public void Remove(string userId, string connectionId)
        {
            if (_userConnections.TryGetValue(userId, out var streamConnections))
            {
                lock (_lockObject)
                {
                    streamConnections.Remove(connectionId);

                    if (_userConnections[userId].Count == 0)
                        _userConnections.Remove(userId);
                }
            }
        }

        public Task WriteToUserAsync(string userId, NotificationOutput output)
        {
            if (_userConnections.TryGetValue(userId, out var streamConnections))
            {
                return streamConnections.WriteToAllConnectionsAsync(output);
            }
            return Task.CompletedTask;
        }
    }
}
