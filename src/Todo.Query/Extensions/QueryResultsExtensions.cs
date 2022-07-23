using Todo.Query.Entities;
using Todo.Query.Server.TodoProto;

namespace Todo.Query.Extensions
{
    public static class QueryResultsExtensions
    {
        public static TaskFilterOutput ToFilterOutput(this TodoTask todoTask) =>
            new()
            {
                Id = todoTask.Id.ToString(),
                UserId = todoTask.UserId,
                Title = todoTask.Title,
                IsCompleted = todoTask.IsCompleted,
                DueDate = todoTask.DueDate.ToUtcTimestamp(),
                DuplicateTitle = !todoTask.IsUniqueTitle
            };

        public static FindResponse ToFindResponse(this TodoTask todoTask) =>
            new()
            {
                Id = todoTask.Id.ToString(),
                UserId = todoTask.UserId,
                Title = todoTask.Title,
                IsCompleted = todoTask.IsCompleted,
                DueDate = todoTask.DueDate.ToUtcTimestamp(),
                Note = todoTask.Note,
                DuplicateTitle = !todoTask.IsUniqueTitle
            };

        public static SimilarTitleExistsResponse ToSimilarTitleResponse(this TodoTask? todoTask) =>
            new()
            {
                Id = todoTask?.Id.ToString(),
                Exists = todoTask != null
            };
    }
}
