using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Todo.Query.Test.Client.TodoProto;
using Todo.Query.Test.Helpers;

namespace Todo.Query.Test.Live.Helpers
{
    public class NotificationsStreamHelper : IDisposable
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly AsyncServerStreamingCall<NotificationResponse> _call;

        public NotificationsStreamHelper(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            var client = new Tasks.TasksClient(_factory.CreateGrpcChannel());
            _call = client.Notifications(new Empty());
            Subscribe();
        }

        public List<NotificationResponse> Notifications { get; } = new List<NotificationResponse>();

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
