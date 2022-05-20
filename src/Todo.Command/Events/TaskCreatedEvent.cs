using Todo.Command.Events.DataTypes;

namespace Todo.Command.Events
{
    public class TaskCreatedEvent : Event<TaskCreatedData>
    {
        public TaskCreatedEvent(
            Guid aggregateId,
            int sequence,
            string userId,
            TaskCreatedData data
        ) : base(aggregateId, sequence, data, userId, EventType.TaskCreated)
        {
        }
    }
}
