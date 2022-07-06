using Todo.Command.Events.DataTypes;

namespace Todo.Command.Events
{
    public class TaskInfoUpdated : Event<TaskInfoUpdatedData>
    {
        public TaskInfoUpdated(
            Guid aggregateId,
            int sequence,
            string userId,
            TaskInfoUpdatedData data
        ) : base(aggregateId, sequence, data, userId)
        {
        }
    }
}
