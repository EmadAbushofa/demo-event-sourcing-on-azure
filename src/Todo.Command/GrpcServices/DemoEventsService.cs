using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Azure.Cosmos;
using Todo.Command.Abstractions;
using Todo.Command.DemoEventsProto;
using Todo.Command.Events;
using Todo.Command.Events.DataTypes;
using Todo.Command.Extensions;

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
                data: new TaskCreatedData(
                    Title: request.Title,
                    DueDate: ProtoExtensions.ToDate(request.DueDate),
                    Note: request.Note
                )
            );

            await AppendToStreamThenDeleteAsync(@event);

            return new Empty();
        }

        public override async Task<Empty> UpdateInfo(UpdateInfoRequest request, ServerCallContext context)
        {
            var @event = new TaskInfoUpdated(
                aggregateId: Guid.Parse(request.Id),
                sequence: request.Sequence,
                userId: request.UserId,
                data: new TaskInfoUpdatedData(
                    Title: request.Title,
                    Note: request.Note
                )
            );

            await AppendToStreamThenDeleteAsync(@event);

            return new Empty();
        }

        public override async Task<Empty> ChangeDueDate(ChangeDueDateRequest request, ServerCallContext context)
        {
            var @event = new TaskDueDateChanged(
                aggregateId: Guid.Parse(request.Id),
                sequence: request.Sequence,
                userId: request.UserId,
                data: new TaskDueDateChangedData(
                    DueDate: ProtoExtensions.ToDate(request.DueDate)
                )
            );

            await AppendToStreamThenDeleteAsync(@event);

            return new Empty();
        }

        public override async Task<Empty> Complete(CompleteRequest request, ServerCallContext context)
        {
            var @event = new TaskCompleted(
                aggregateId: Guid.Parse(request.Id),
                sequence: request.Sequence,
                userId: request.UserId,
                data: new object()
            );

            await AppendToStreamThenDeleteAsync(@event);

            return new Empty();
        }

        public override async Task<Empty> Uncomplete(CompleteRequest request, ServerCallContext context)
        {
            var @event = new TaskUncompleted(
                aggregateId: Guid.Parse(request.Id),
                sequence: request.Sequence,
                userId: request.UserId,
                data: new object()
            );

            await AppendToStreamThenDeleteAsync(@event);

            return new Empty();
        }

        private async Task AppendToStreamThenDeleteAsync(Event @event)
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
