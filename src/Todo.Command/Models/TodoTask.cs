using Todo.Command.CommandHandlers.ChangeDueDate;
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
        private DateTime DueDate { get; set; }

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
            DueDate = @event.Data.DueDate;
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

        private void Mutate(TaskInfoUpdated @event)
        {
            Title = @event.Data.Title;
            Note = @event.Data.Note;
        }

        public void ChangeDueDate(ChangeDueDateCommand command)
        {
            if (UserId != command.UserId)
                throw new NotFoundException();

            if (DueDate == command.DueDate)
                return;

            var @event = command.ToEvent(NextSequence);

            ApplyNewChange(@event);
        }

        private void Mutate(TaskDueDateChanged @event)
        {
            DueDate = @event.Data.DueDate;
        }

        protected override void Mutate(Event @event)
        {
            switch (@event)
            {
                case TaskCreated taskCreated:
                    Mutate(taskCreated);
                    break;

                case TaskInfoUpdated taskInfoUpdated:
                    Mutate(taskInfoUpdated);
                    break;

                case TaskDueDateChanged taskDueDateChanged:
                    Mutate(taskDueDateChanged);
                    break;

                default:
                    break;
            }
        }
    }
}
