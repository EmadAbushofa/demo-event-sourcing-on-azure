using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.Features.Create
{
    public class TaskCreatedEventHandler : IRequestHandler<TaskCreatedEvent, bool>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TaskCreatedEventHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(TaskCreatedEvent @event, CancellationToken cancellationToken)
        {
            var task = TodoTask.FromCreatedEvent(@event);

            await _unitOfWork.Tasks.AddAsync(task);

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
