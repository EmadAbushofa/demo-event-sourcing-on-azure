using Todo.Command.Events.DataTypes;

namespace Todo.Command.Events
{
    public class TaskCreatedEvent : Event<TaskCreatedData>
    {
        public TaskCreatedEvent(
            string userId,
            TaskCreatedData data
        ) : base(aggregateId: Guid.NewGuid(), sequence: 1, data, userId, EventType.TaskCreated)
        {
        }
    }
}
