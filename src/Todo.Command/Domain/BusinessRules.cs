using Todo.Command.CommandHandlers.ChangeDueDate;
using Todo.Command.CommandHandlers.Complete;
using Todo.Command.CommandHandlers.Delete;
using Todo.Command.CommandHandlers.Uncomplete;
using Todo.Command.CommandHandlers.UpdateInfo;

namespace Todo.Command.Domain
{
    public class BusinessRules
    {
        private readonly BusinessRequirements _requirements;

        public BusinessRules(TodoTask todoTask)
        {
            _requirements = new BusinessRequirements(todoTask);
        }

        public void Validate(UpdateTaskInfoCommand command)
        {
            _requirements.RequireNotDeleted();
            _requirements.RequireSameUser(command);
        }

        public void Validate(ChangeDueDateCommand command)
        {
            _requirements.RequireNotDeleted();
            _requirements.RequireSameUser(command);
        }

        public void Validate(CompleteCommand command)
        {
            _requirements.RequireNotDeleted();
            _requirements.RequireSameUser(command);
            _requirements.RequireNotCompleted();
        }

        public void Validate(UncompleteCommand command)
        {
            _requirements.RequireNotDeleted();
            _requirements.RequireSameUser(command);
            _requirements.RequireCompleted();
        }

        public void Validate(DeleteCommand command)
        {
            _requirements.RequireNotDeleted();
            _requirements.RequireSameUser(command);
        }
    }
}
