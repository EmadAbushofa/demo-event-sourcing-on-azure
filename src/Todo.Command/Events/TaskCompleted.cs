namespace Todo.Command.Events
{
    public class TaskCompleted : Event<object>
    {
        public TaskCompleted(
            Guid aggregateId,
            int sequence,
            string userId,
            object data
        ) : base(aggregateId, sequence, data, userId)
        {
        }
    }
}
