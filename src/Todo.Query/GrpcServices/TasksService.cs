using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using Todo.Query.QueryHandlers.Find;
using Todo.Query.QueryHandlers.SimilarTitleCheck;
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

        public override async Task<FindResponse> Find(FindRequest request, ServerCallContext context)
        {
            var query = new FindQuery(Guid.Parse(request.Id));

            var result = await _mediator.Send(query);

            return new FindResponse()
            {
                Id = result.Id.ToString(),
                UserId = result.UserId,
                Title = result.Title,
                DueDate = result.DueDate.ToUniversalTime().Date.ToTimestamp(),
                Note = result.Note,
                IsCompleted = result.IsCompleted
            };
        }

        public override async Task<SimilarTitleExistsResponse> SimilarTitleExists(SimilarTitleExistsRequest request, ServerCallContext context)
        {
            var query = new SimilarTitleQuery(UserId: request.UserId, Title: request.Title);

            var result = await _mediator.Send(query);

            return new SimilarTitleExistsResponse()
            {
                Id = result?.Id.ToString(),
                Exists = result != null
            };
        }
    }
}