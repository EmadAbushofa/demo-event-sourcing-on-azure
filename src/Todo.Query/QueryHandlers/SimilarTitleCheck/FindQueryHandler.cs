using MediatR;
using Todo.Query.Abstractions;
using Todo.Query.Entities;

namespace Todo.Query.QueryHandlers.SimilarTitleCheck
{
    public class SimilarTitleQueryHandler : IRequestHandler<SimilarTitleQuery, TodoTask?>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SimilarTitleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TodoTask?> Handle(SimilarTitleQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.Tasks.GetSimilarTodoTaskAsync(
                userId: request.UserId,
                title: request.Title
            );
        }
    }
}
