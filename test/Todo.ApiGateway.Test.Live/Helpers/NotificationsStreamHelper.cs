using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Todo.ApiGateway.Test.Live.TodoProto.Channel;

namespace Todo.ApiGateway.Test.Helpers
{
    public class NotificationsStreamHelper : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly AsyncServerStreamingCall<NotificationOutput> _call;

        public NotificationsStreamHelper(WebApplicationFactory<Program> factory, string userId)
        {
            _factory = factory;
            var client = new TasksChannel.TasksChannelClient(_factory.CreateGrpcChannel(userId));
            _call = client.Notifications(new Empty());
            Subscribe();
        }

        public List<NotificationOutput> Notifications { get; } = new List<NotificationOutput>();

        private async void Subscribe()
        {
            await foreach (var response in _call.ResponseStream.ReadAllAsync())
            {
                Notifications.Add(response);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _call.Dispose();
            }
        }
    }
}
