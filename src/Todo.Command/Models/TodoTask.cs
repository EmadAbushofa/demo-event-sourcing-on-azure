using Todo.Command.CommandHandlers.ChangeDueDate;
using Todo.Command.CommandHandlers.Complete;
using Todo.Command.CommandHandlers.Create;
using Todo.Command.CommandHandlers.Delete;
using Todo.Command.CommandHandlers.Uncomplete;
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
        private bool IsCompleted { get; set; }
        private bool IsDeleted { get; set; }

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

        public void Complete(CompleteCommand command)
        {
            if (UserId != command.UserId)
                throw new NotFoundException();

            if (IsCompleted)
                throw new RuleViolationException("Task is already completed.");

            var @event = command.ToEvent(NextSequence);

            ApplyNewChange(@event);
        }

        private void Mutate(TaskCompleted _)
        {
            IsCompleted = true;
        }

        public void Uncomplete(UncompleteCommand command)
        {
            if (UserId != command.UserId)
                throw new NotFoundException();

            if (!IsCompleted)
                throw new RuleViolationException("Task is already uncompleted.");

            var @event = command.ToEvent(NextSequence);

            ApplyNewChange(@event);
        }

        private void Mutate(TaskUncompleted _)
        {
            IsCompleted = false;
        }

        public void Delete(DeleteCommand command)
        {
            if (UserId != command.UserId)
                throw new NotFoundException();

            if (IsDeleted)
                throw new NotFoundException();

            var @event = command.ToEvent(NextSequence);

            ApplyNewChange(@event);
        }

        private void Mutate(TaskDeleted _)
        {
            IsDeleted = true;
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

                case TaskCompleted taskCompleted:
                    Mutate(taskCompleted);
                    break;

                case TaskUncompleted taskUncompleted:
                    Mutate(taskUncompleted);
                    break;

                case TaskDeleted taskDeleted:
                    Mutate(taskDeleted);
                    break;

                default:
                    break;
            }
        }
    }
}
