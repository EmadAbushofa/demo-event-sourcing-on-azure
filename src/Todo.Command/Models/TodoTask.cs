using Todo.Command.CommandHandlers.Create;
using Todo.Command.Events;
using Todo.Command.Extensions;

namespace Todo.Command.Models
{
    public class TodoTask : Aggregate<TodoTask>, IAggregate
    {
        public static TodoTask Create(CreateTaskCommand command)
        {
            var @event = command.ToEvent();

            var todoTask = new TodoTask();

            todoTask.ApplyNewChange(@event);

            return todoTask;
        }

        protected override void Mutate(Event @event)
        {
            switch (@event)
            {
                case TaskCreatedEvent taskCreated:
                    Mutate(taskCreated);
                    break;

                default:
                    break;
            }
        }

        private void Mutate(TaskCreatedEvent @event)
        {

        }
    }
}
