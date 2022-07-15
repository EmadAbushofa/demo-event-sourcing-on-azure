namespace Todo.Command.Events
{
    public class TaskDeleted : Event<object>
    {
        public TaskDeleted(
            Guid aggregateId,
            int sequence,
            string userId,
            object data
        ) : base(aggregateId, sequence, data, userId)
        {
        }
    }
}
