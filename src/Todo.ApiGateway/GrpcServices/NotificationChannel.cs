using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Todo.ApiGateway.TodoProto.Channel;
using Todo.ApiGateway.TodoProto.Query;

namespace Todo.ApiGateway.GrpcServices
{
    public class NotificationChannel
    {
        private readonly Dictionary<string, IAsyncStreamWriter<NotificationOutput>> _usersStreams = new();
        private readonly NotificationHandler _notificationHandler = new();

        public NotificationChannel(Tasks.TasksClient tasksClient)
        {
            var call = tasksClient.Notifications(new Empty());
            ReaderStream = call.ResponseStream;
            StartReading();
        }

        public IAsyncStreamReader<NotificationResponse> ReaderStream { get; private set; }

        private async void StartReading()
        {
            await foreach (var response in ReaderStream.ReadAllAsync())
            {
                var notification = _notificationHandler.Handle(response);

                if (notification == null)
                    continue;

                await WriteToAsync(response.Task.UserId, notification);
            }
        }

        private async Task WriteToAsync(string userId, NotificationOutput output)
        {
            if (_usersStreams.TryGetValue(userId, out var userStream))
            {
                await userStream.WriteAsync(output);
            }
        }

        public void AddUser(string userId, IAsyncStreamWriter<NotificationOutput> stream) =>
            _usersStreams.TryAdd(userId, stream);

        public void RemoveUser(string userId) => _usersStreams.Remove(userId);
    }
}
