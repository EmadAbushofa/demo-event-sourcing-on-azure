using Todo.Command.Events.DataTypes;

namespace Todo.Command.Events
{
    public class TaskCreated : Event<TaskCreatedData>
    {
        public TaskCreated(
            Guid aggregateId,
            int sequence,
            string userId,
            TaskCreatedData data
        ) : base(aggregateId, sequence, data, userId)
        {
        }
    }
}
