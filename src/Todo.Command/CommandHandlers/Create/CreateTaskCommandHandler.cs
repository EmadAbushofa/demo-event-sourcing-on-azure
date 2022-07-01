using MediatR;
using Todo.Command.Abstractions;
using Todo.Command.Exceptions;
using Todo.Command.Models;

namespace Todo.Command.CommandHandlers.Create
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, Guid>
    {
        private readonly IEventStore _eventStore;
        private readonly ITodoQuery _query;

        public CreateTaskCommandHandler(IEventStore eventStore, ITodoQuery query)
        {
            _eventStore = eventStore;
            _query = query;
        }

        public async Task<Guid> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
        {
            if (await _query.SimilarTitleExistsAsync(
                userId: command.UserId,
                title: command.Title
            ))
                throw new AlreadyExistsException("Similar task with the same title exists.");

            var todoTask = TodoTask.Create(command);

            await _eventStore.AppendToStreamAsync(todoTask);

            return todoTask.Id;
        }
    }
}
