using MediatR;
using Todo.Query.Abstractions;
using Todo.Query.Entities;

namespace Todo.Query.EventHandlers.Created
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
            if (await _unitOfWork.Tasks.ExistsAsync(@event.AggregateId))
                return true;

            var similarTitleExists = await _unitOfWork.Tasks.HasSimilarTodoTaskAsync(
                userId: @event.UserId,
                title: @event.Data.Title
            );

            var task = similarTitleExists
                ? TodoTask.FromCreatedEvent(@event, isUniqueTitle: false)
                : TodoTask.FromCreatedEvent(@event, isUniqueTitle: true);

            await _unitOfWork.Tasks.AddAsync(task);

            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}
