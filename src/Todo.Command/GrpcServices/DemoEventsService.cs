using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Todo.Command.Abstraction;
using Todo.Command.Events;
using Todo.Command.Server.DemoEventsProto;

namespace Todo.Command.GrpcServices
{
    public class DemoEventsService : DemoEvents.DemoEventsBase
    {
        private readonly IEventStore _eventStore;

        public DemoEventsService(IEventStore eventStore, IWebHostEnvironment environment)
        {
            if (environment.IsDevelopment() == false)
                throw new InvalidOperationException("Cannot send demo requests in non development environment.");

            _eventStore = eventStore;
        }


        public override async Task<Empty> Create(CreateRequest request, ServerCallContext context)
        {
            var @event = new TaskCreatedEvent(
                aggregateId: Guid.Parse(request.Id),
                sequence: 1,
                userId: request.UserId,
                data: new Events.DataTypes.TaskCreatedData(
                    Title: request.Title,
                    DueDate: request.DueDate.ToDateTime(),
                    Note: request.Note
                )
            );

            await _eventStore.AppendToStreamAsync(new Event[] { @event }, @event.AggregateId);

            return new Empty();
        }
    }
}
