using Todo.Query.QueryHandlers.Filter;
using Todo.Query.QueryHandlers.Find;
using Todo.Query.QueryHandlers.SimilarTitleCheck;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.Extensions
{
    public static class QueryRequestsExtensions
    {
        public static FilterQuery ToQuery(this FilterRequest request)
            => new(
                Page: request.Page ?? 1,
                Size: request.Size ?? 25,
                IsCompleted: request.IsCompleted,
                UserId: request.UserId,
                DueDateFrom: request.DueDateFrom?.ToDate(),
                DueDateTo: request.DueDateTo?.ToDate()
            );

        public static FindQuery ToQuery(this FindRequest request)
            => new(Id: Guid.Parse(request.Id));

        public static SimilarTitleQuery ToQuery(this SimilarTitleExistsRequest request)
            => new(
                UserId: request.UserId,
                Title: request.Title,
                ExcludedId: Guid.TryParse(request.ExcludedId, out var excludedId) ? excludedId : null
            );
    }
}
