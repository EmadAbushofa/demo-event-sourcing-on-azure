using Grpc.Net.Client;
using Todo.Query.Test.Live.Client.DemoEventsProto;

namespace Todo.Query.Test.Live.Helpers
{
    public static class CommandServiceHelper
    {
        public static DemoEvents.DemoEventsClient CreateDemoEventsClient()
        {
            var channel = GrpcChannel.ForAddress(LiveTestConfig.CommandServiceUrl);

            return new DemoEvents.DemoEventsClient(channel);
        }
    }
}