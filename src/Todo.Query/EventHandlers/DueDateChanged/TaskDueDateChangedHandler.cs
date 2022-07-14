using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.EventHandlers.DueDateChanged
{
    public class TaskDueDateChangedHandler : IRequestHandler<TaskDueDateChanged, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskDueDateChangedHandler> _logger;

        public TaskDueDateChangedHandler(IUnitOfWork unitOfWork, ILogger<TaskDueDateChangedHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(TaskDueDateChanged @event, CancellationToken cancellationToken)
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

            todoTask.Apply(@event);

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
