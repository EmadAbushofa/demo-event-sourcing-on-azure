using Grpc.Core;
using MediatR;
using Todo.Query.Extensions;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.GrpcServices
{
    public class TasksService : Tasks.TasksBase
    {
        private readonly IMediator _mediator;

        public TasksService(IMediator mediator)
        {
            _mediator = mediator;
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
    }
}