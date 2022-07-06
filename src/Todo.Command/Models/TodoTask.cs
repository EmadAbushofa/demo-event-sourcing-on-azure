using Todo.Command.CommandHandlers.Create;
using Todo.Command.CommandHandlers.UpdateInfo;
using Todo.Command.Events;
using Todo.Command.Exceptions;
using Todo.Command.Extensions;

namespace Todo.Command.Models
{
    public class TodoTask : Aggregate<TodoTask>, IAggregate
    {
        private string? UserId { get; set; }
        private string? Title { get; set; }
        private string? Note { get; set; }

        public static TodoTask Create(CreateTaskCommand command)
        {
            var @event = command.ToEvent();

            var todoTask = new TodoTask();

            todoTask.ApplyNewChange(@event);

            return todoTask;
        }

        private void Mutate(TaskCreated @event)
        {
            UserId = @event.UserId;
            Title = @event.Data.Title;
            Note = @event.Data.Note;
        }

        public void UpdateInfo(UpdateTaskInfoCommand command)
        {
            if (UserId != command.UserId)
                throw new NotFoundException();

            if (Title == command.Title && Note == command.Note)
                return;

            var @event = command.ToEvent(NextSequence);

            ApplyNewChange(@event);
        }


        protected override void Mutate(Event @event)
        {
            switch (@event)
            {
                case TaskCreated taskCreated:
                    Mutate(taskCreated);
                    break;

                default:
                    break;
            }
        }
    }
}
