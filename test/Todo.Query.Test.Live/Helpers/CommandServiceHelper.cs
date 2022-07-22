using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using Todo.Query.EventHandlers.Completed;
using Todo.Query.EventHandlers.Created;
using Todo.Query.EventHandlers.Deleted;
using Todo.Query.EventHandlers.DueDateChanged;
using Todo.Query.EventHandlers.InfoUpdated;
using Todo.Query.EventHandlers.Uncompleted;
using Todo.Query.Extensions;
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

        public static AsyncUnaryCall<Empty> SendAsync(TaskCreated @event)
        {
            var client = CreateDemoEventsClient();
            return client.CreateAsync(new CreateRequest()
            {
                Id = @event.AggregateId.ToString(),
                DueDate = @event.Data.DueDate.ToUtcTimestamp(),
                Note = @event.Data.Note,
                Title = @event.Data.Title,
                UserId = @event.UserId,
            });
        }

        public static AsyncUnaryCall<Empty> SendAsync(TaskCompleted @event)
        {
            var client = CreateDemoEventsClient();
            return client.CompleteAsync(new CompleteRequest()
            {
                Id = @event.AggregateId.ToString(),
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });
        }

        public static AsyncUnaryCall<Empty> SendAsync(TaskInfoUpdated @event)
        {
            var client = CreateDemoEventsClient();
            return client.UpdateInfoAsync(new UpdateInfoRequest()
            {
                Id = @event.AggregateId.ToString(),
                Note = @event.Data.Note,
                Title = @event.Data.Title,
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });
        }

        public static AsyncUnaryCall<Empty> SendAsync(TaskDueDateChanged @event)
        {
            var client = CreateDemoEventsClient();
            return client.ChangeDueDateAsync(new ChangeDueDateRequest()
            {
                Id = @event.AggregateId.ToString(),
                DueDate = @event.Data.DueDate.ToUtcTimestamp(),
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });
        }

        public static AsyncUnaryCall<Empty> SendAsync(TaskUncompleted @event)
        {
            var client = CreateDemoEventsClient();
            return client.UncompleteAsync(new CompleteRequest()
            {
                Id = @event.AggregateId.ToString(),
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });
        }

        public static AsyncUnaryCall<Empty> SendAsync(TaskDeleted @event)
        {
            var client = CreateDemoEventsClient();
            return client.DeleteAsync(new DeleteRequest()
            {
                Id = @event.AggregateId.ToString(),
                UserId = @event.UserId,
                Sequence = @event.Sequence,
            });
        }
    }
}