using MediatR;
using Todo.Query.Abstractions;
using Todo.Query.Entities;
using Todo.Query.Exceptions;

namespace Todo.Query.QueryHandlers.Find
{
    public class FindQueryHandler : IRequestHandler<FindQuery, TodoTask>
    {
        private readonly IUnitOfWork _unitOfWork;

        public FindQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TodoTask> Handle(FindQuery request, CancellationToken cancellationToken)
        {
            var todoTask = await _unitOfWork.Tasks.FindAsync(request.Id);

            if (todoTask == null)
                throw new NotFoundException("Task not found.");

            return todoTask;
        }
    }
}
