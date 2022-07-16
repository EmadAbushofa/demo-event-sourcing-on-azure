using MediatR;
using Todo.Query.Abstractions;
using Todo.Query.Entities;

namespace Todo.Query.EventHandlers.Created
{
    public class TaskCreatedHandler : IRequestHandler<TaskCreated, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public TaskCreatedHandler(IUnitOfWork unitOfWork, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task<bool> Handle(TaskCreated @event, CancellationToken cancellationToken)
        {
            if (await _unitOfWork.Tasks.ExistsAsync(@event.AggregateId, cancellationToken))
                return true;

            var similarTitleExists = await _unitOfWork.Tasks.HasSimilarTodoTaskAsync(
                userId: @event.UserId,
                title: @event.Data.Title,
                cancellationToken
            );

            var task = similarTitleExists
                ? TodoTask.FromCreatedEvent(@event, isUniqueTitle: false)
                : TodoTask.FromCreatedEvent(@event, isUniqueTitle: true);

            await _unitOfWork.Tasks.AddAsync(task);

            await _unitOfWork.CompleteAsync(cancellationToken);

            await _mediator.Publish(new EventConsumed(@event, task));

            return true;
        }
    }
}
