using Todo.Command.Events.DataTypes;

namespace Todo.Command.Events
{
    public class TaskInfoUpdatedEvent : Event<TaskInfoUpdatedData>
    {
        public TaskInfoUpdatedEvent(
            Guid aggregateId,
            int sequence,
            string userId,
            TaskInfoUpdatedData data
        ) : base(aggregateId, sequence, data, userId, EventType.TaskInfoUpdated)
        {
        }
    }
}
