using Todo.Command.CommandHandlers.ChangeDueDate;
using Todo.Command.CommandHandlers.UpdateInfo;

namespace Todo.Command.Domain
{
    public class ChangeDetector
    {
        private readonly TodoTask _todoTask;

        public ChangeDetector(TodoTask todoTask)
        {
            _todoTask = todoTask;
        }

        public bool HaveDifferentValues(UpdateTaskInfoCommand command) =>
            _todoTask.Title != command.Title || _todoTask.Note != command.Note;

        public bool HaveDifferentValues(ChangeDueDateCommand command) =>
            _todoTask.DueDate != command.DueDate;
    }
}
