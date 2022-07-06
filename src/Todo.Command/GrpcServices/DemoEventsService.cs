using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Azure.Cosmos;
using Todo.Command.Abstractions;
using Todo.Command.DemoEventsProto;
using Todo.Command.Events;

namespace Todo.Command.GrpcServices
{
    public class DemoEventsService : DemoEvents.DemoEventsBase
    {
        private readonly IEventStore _eventStore;
        private readonly Container _container;

        public DemoEventsService(
            IEventStore eventStore,
            IWebHostEnvironment environment,
            Container container
        )
        {
            if (environment.IsDevelopment() == false)
                throw new InvalidOperationException("Cannot send demo requests in non development environment.");

            _eventStore = eventStore;
            _container = container;
        }


        public override async Task<Empty> Create(CreateRequest request, ServerCallContext context)
        {
            var @event = new TaskCreated(
                aggregateId: Guid.Parse(request.Id),
                sequence: 1,
                userId: request.UserId,
                data: new Events.DataTypes.TaskCreatedData(
                    Title: request.Title,
                    DueDate: request.DueDate.ToDateTime(),
                    Note: request.Note
                )
            );

            await AppendToStreamThenDeleteAsync(@event);

            return new Empty();
        }

        private async Task AppendToStreamThenDeleteAsync(TaskCreated @event)
        {
            await _eventStore.AppendToStreamAsync(@event);

            await Task.Delay(5000);

            await _container.DeleteItemStreamAsync(
                id: @event.Sequence.ToString(),
                partitionKey: new PartitionKey(@event.AggregateId.ToString())
            );
        }
    }
}
