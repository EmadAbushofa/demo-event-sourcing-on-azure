namespace Todo.Command.Events
{
    public class TaskUncompleted : Event<object>
    {
        public TaskUncompleted(
            Guid aggregateId,
            int sequence,
            string userId,
            object data
        ) : base(aggregateId, sequence, data, userId)
        {
        }
    }
}
