using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.EventHandlers.InfoUpdated
{
    public class TaskInfoUpdatedHandler : IRequestHandler<TaskInfoUpdated, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskInfoUpdatedHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<bool> Handle(TaskInfoUpdated @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
