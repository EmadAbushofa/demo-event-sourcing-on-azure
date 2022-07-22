using MediatR;
using Todo.Query.Abstractions;

namespace Todo.Query.EventHandlers.InfoUpdated
{
    public class TaskInfoUpdatedHandler : IRequestHandler<TaskInfoUpdated, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<TaskInfoUpdatedHandler> _logger;

        public TaskInfoUpdatedHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<TaskInfoUpdatedHandler> logger
        )
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<bool> Handle(TaskInfoUpdated @event, CancellationToken cancellationToken)
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

            if (todoTask.IsCompleted)
            {
                todoTask.Apply(@event);
            }
            else
            {
                var isUniquetitle = !await _unitOfWork.Tasks.HasSimilarTodoTaskAsync(
                    userId: @event.UserId,
                    title: @event.Data.Title,
                    cancellationToken
                );

                todoTask.Apply(@event, isUniquetitle);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _mediator.Publish(new EventConsumed(@event, todoTask));
            return true;
        }
    }
}
