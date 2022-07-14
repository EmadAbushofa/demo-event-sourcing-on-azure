using Todo.Command.Events.DataTypes;

namespace Todo.Command.Events
{
    public class TaskDueDateChanged : Event<TaskDueDateChangedData>
    {
        public TaskDueDateChanged(
            Guid aggregateId,
            int sequence,
            string userId,
            TaskDueDateChangedData data
        ) : base(aggregateId, sequence, data, userId)
        {
        }
    }
}
