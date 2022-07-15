using Todo.Command.CommandHandlers;
using Todo.Command.Exceptions;

namespace Todo.Command.Domain
{
    public class BusinessRequirements
    {
        private readonly TodoTask _todoTask;

        public BusinessRequirements(TodoTask todoTask)
        {
            _todoTask = todoTask;
        }

        public void RequireSameUser(ITodoCommand command)
        {
            if (_todoTask.UserId != command.UserId)
                throw new NotFoundException();
        }

        public void RequireNotDeleted()
        {
            if (_todoTask.IsDeleted)
                throw new NotFoundException();
        }

        public void RequireCompleted()
        {
            if (!_todoTask.IsCompleted)
                throw new RuleViolationException("Task is already uncompleted.");
        }

        public void RequireNotCompleted()
        {
            if (_todoTask.IsCompleted)
                throw new RuleViolationException("Task is already completed.");
        }
    }
}
