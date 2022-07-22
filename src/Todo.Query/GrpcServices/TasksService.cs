using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Todo.Query.Extensions;
using Todo.Query.Server.TodoProto;
using Todo.Query.Services;

namespace Todo.Query.GrpcServices
{
    public class TasksService : Tasks.TasksBase
    {
        private readonly IMediator _mediator;
        private readonly NotificationsStreamService _notificationsStreamService;

        public TasksService(IMediator mediator, NotificationsStreamService notificationsStreamService)
        {
            _mediator = mediator;
            _notificationsStreamService = notificationsStreamService;
        }

        public override async Task<FilterResponse> Filter(FilterRequest request, ServerCallContext context)
        {
            var query = request.ToQuery();

            var result = await _mediator.Send(query, context.CancellationToken);

            var outputs = result.Tasks.Select(t => t.ToFilterOutput());

            return new FilterResponse()
            {
                Page = result.Page,
                Size = result.Size,
                Total = result.Total,
                Tasks =
                {
                    outputs
                }
            };
        }

        public override async Task<FindResponse> Find(FindRequest request, ServerCallContext context)
        {
            var query = request.ToQuery();

            var result = await _mediator.Send(query, context.CancellationToken);

            return result.ToFindResponse();
        }

        public override async Task<SimilarTitleExistsResponse> SimilarTitleExists(SimilarTitleExistsRequest request, ServerCallContext context)
        {
            var query = request.ToQuery();

            var result = await _mediator.Send(query, context.CancellationToken);

            return result.ToSimilarTitleResponse();
        }

        public override async Task Notifications(
            Empty request,
            IServerStreamWriter<NotificationResponse> responseStream,
            ServerCallContext context
        )
        {
            _notificationsStreamService.AddStream(responseStream);

            while (!context.CancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000);
            }
        }
    }
}