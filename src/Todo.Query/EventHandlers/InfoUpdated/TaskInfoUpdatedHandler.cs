using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.EventHandlers.InfoUpdated
{
    public class TaskInfoUpdatedHandler : IRequestHandler<TaskInfoUpdated, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskInfoUpdatedHandler> _logger;

        public TaskInfoUpdatedHandler(IUnitOfWork unitOfWork, ILogger<TaskInfoUpdatedHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(TaskInfoUpdated @event, CancellationToken cancellationToken)
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

            if (todoTask.IsCompleted)
            {
                todoTask.Apply(@event);
            }
            else
            {
                var isUniquetitle = !await _unitOfWork.Tasks.HasSimilarTodoTaskAsync(
                    userId: @event.UserId,
                    title: @event.Data.Title
                );

                todoTask.Apply(@event, isUniquetitle);
            }

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
