using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.EventHandlers.Deleted
{
    public class TaskDeletedHandler : IRequestHandler<TaskDeleted, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskDeletedHandler> _logger;

        public TaskDeletedHandler(IUnitOfWork unitOfWork, ILogger<TaskDeletedHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(TaskDeleted @event, CancellationToken cancellationToken)
        {
            var todoTask = await _unitOfWork.Tasks.FindAsync(@event.AggregateId);

            if (todoTask == null || todoTask.Sequence < @event.Sequence - 1)
            {
                _logger.LogWarning(
                    "Event not handled, AggregateId: {AggregateId}, Sequence: {Sequence}.",
                    @event.AggregateId,
                    @event.Sequence
                );
                return false;
            }

            if (todoTask.Sequence >= @event.Sequence)
                return true;

            await _unitOfWork.Tasks.RemoveAsync(todoTask);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
