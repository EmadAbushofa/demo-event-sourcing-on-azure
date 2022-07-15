using Todo.Command.Abstractions.Domain;
using Todo.Command.CommandHandlers.ChangeDueDate;
using Todo.Command.CommandHandlers.Complete;
using Todo.Command.CommandHandlers.Create;
using Todo.Command.CommandHandlers.Delete;
using Todo.Command.CommandHandlers.Uncomplete;
using Todo.Command.CommandHandlers.UpdateInfo;
using Todo.Command.Events;
using Todo.Command.Extensions;

namespace Todo.Command.Domain
{
    public class TodoTask : Aggregate<TodoTask>, IAggregate
    {
        private readonly BusinessRules _businessRules;
        private readonly ChangeDetector _changeDetector;
        private TodoTask()
        {
            _businessRules = new BusinessRules(this);
            _changeDetector = new ChangeDetector(this);
        }

        public string? UserId { get; private set; }
        public bool IsCompleted { get; private set; }
        public bool IsDeleted { get; private set; }
        public string? Title { get; private set; }
        public string? Note { get; private set; }
        public DateTime DueDate { get; private set; }

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
            _businessRules.Validate(command);

            if (_changeDetector.HaveDifferentValues(command))
            {
                var @event = command.ToEvent(NextSequence);

                ApplyNewChange(@event);
            }
        }

        private void Mutate(TaskInfoUpdated @event)
        {
            Title = @event.Data.Title;
            Note = @event.Data.Note;
        }

        public void ChangeDueDate(ChangeDueDateCommand command)
        {
            _businessRules.Validate(command);

            if (_changeDetector.HaveDifferentValues(command))
            {
                var @event = command.ToEvent(NextSequence);

                ApplyNewChange(@event);
            }
        }

        private void Mutate(TaskDueDateChanged @event)
        {
            DueDate = @event.Data.DueDate;
        }

        public void Complete(CompleteCommand command)
        {
            _businessRules.Validate(command);

            var @event = command.ToEvent(NextSequence);

            ApplyNewChange(@event);
        }

        private void Mutate(TaskCompleted _)
        {
            IsCompleted = true;
        }

        public void Uncomplete(UncompleteCommand command)
        {
            _businessRules.Validate(command);

            var @event = command.ToEvent(NextSequence);

            ApplyNewChange(@event);
        }

        private void Mutate(TaskUncompleted _)
        {
            IsCompleted = false;
        }

        public void Delete(DeleteCommand command)
        {
            _businessRules.Validate(command);

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
