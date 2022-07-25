using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Todo.ApiGateway.TodoProto.Channel;
using Todo.ApiGateway.TodoProto.Query;

namespace Todo.ApiGateway.GrpcServices.Stream
{
    public class NotificationChannel : IHostedService
    {
        private readonly Dictionary<string, IAsyncStreamWriter<NotificationOutput>> _usersStreams = new();
        private readonly NotificationHandler _notificationHandler = new();
        private readonly IServiceProvider _provider;
        private readonly ILogger<NotificationChannel> _logger;

        public NotificationChannel(IServiceProvider provider, ILogger<NotificationChannel> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        private async void StartReading(CancellationToken cancellationToken)
        {
            try
            {
                var tasksClient = _provider.GetRequiredService<Tasks.TasksClient>();
                using var call = tasksClient.Notifications(new Empty(), cancellationToken: cancellationToken);

                await foreach (var response in call.ResponseStream.ReadAllAsync(cancellationToken))
                {
                    var notification = _notificationHandler.Handle(response);

                    if (notification == null)
                        continue;

                    await WriteToAsync(response.Task.UserId, notification);
                }
            }
            catch (RpcException e)
            {
                _logger.LogError(e, "Connection Lost, will try again after 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                StartReading(cancellationToken);
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

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartReading(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
