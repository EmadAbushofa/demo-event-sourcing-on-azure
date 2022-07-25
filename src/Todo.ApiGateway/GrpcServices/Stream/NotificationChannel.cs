using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Todo.ApiGateway.TodoProto.Channel;
using Todo.ApiGateway.TodoProto.Query;

namespace Todo.ApiGateway.GrpcServices.Stream
{
    public class NotificationChannel : IHostedService
    {
        private readonly NotificationHandler _notificationHandler = new();
        private readonly IServiceProvider _provider;
        private readonly ILogger<NotificationChannel> _logger;
        private readonly UserStreamConnections _userStreamConnections = new();

        public NotificationChannel(IServiceProvider provider, ILogger<NotificationChannel> logger)
        {
            _provider = provider;
            _logger = logger;
        }

        private async Task StartReadingAsync(CancellationToken cancellationToken)
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

                    await WriteToSpecifiedUserAsync(response.Task.UserId, notification);
                }
            }
            catch (RpcException e)
            {
                _logger.LogError(e, "Connection Lost, will try again after 10 seconds.");
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
                StartReadingAsync(cancellationToken).GetAwaiter();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Connection Error.");
            }
        }

        private Task WriteToSpecifiedUserAsync(string userId, NotificationOutput output)
        {
            _logger.LogInformation("Start writing to user {UserId}", userId);
            return _userStreamConnections.WriteToUserAsync(userId, output)
                .ContinueWith((t) =>
                {
                    _logger.LogInformation("End writing to user {UserId}", userId);
                    return t;
                });
        }

        public void AddUser(string userId, string connectionId, IAsyncStreamWriter<NotificationOutput> stream)
        {
            _logger.LogInformation("User {UserId} connected.", userId);
            _userStreamConnections.Add(userId, connectionId, stream);
        }

        public void RemoveUser(string userId, string connectionId)
        {
            _userStreamConnections.Remove(userId, connectionId);
            _logger.LogInformation("User {UserId} removed a connection.", userId);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartReadingAsync(cancellationToken).GetAwaiter();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
