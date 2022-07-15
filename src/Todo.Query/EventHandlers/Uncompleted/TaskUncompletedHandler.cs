using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.EventHandlers.Uncompleted
{
    public class TaskUncompletedHandler : IRequestHandler<TaskUncompleted, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskUncompletedHandler> _logger;

        public TaskUncompletedHandler(IUnitOfWork unitOfWork, ILogger<TaskUncompletedHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(TaskUncompleted @event, CancellationToken cancellationToken)
        {
            var todoTask = await _unitOfWork.Tasks.FindAsync(@event.AggregateId, cancellationToken);

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

            todoTask.Apply(@event);

            await _unitOfWork.CompleteAsync(cancellationToken);

            return true;
        }
    }
}
