using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;
using Todo.ApiGateway.Extensions;
using Todo.ApiGateway.GrpcServices.Stream;
using Todo.ApiGateway.TodoProto.Channel;

namespace Todo.ApiGateway.GrpcServices
{
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
    public class TasksChannelService : TasksChannel.TasksChannelBase
    {
        private readonly NotificationChannel _channel;

        public TasksChannelService(NotificationChannel channel)
        {
            _channel = channel;
        }

        public override async Task Notifications(
            Empty request,
            IServerStreamWriter<NotificationOutput> responseStream,
            ServerCallContext context
        )
        {
            var userId = context.GetHttpContext().User.GetId();
            _channel.AddUser(userId, responseStream);
            while (!context.CancellationToken.IsCancellationRequested) await Task.Delay(1000);
            _channel.RemoveUser(userId);
        }
    }
}
