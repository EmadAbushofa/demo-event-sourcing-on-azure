namespace Todo.Query.EventHandlers.DueDateChanged
{
    public record TaskDueDateChangedData(
        DateOnly DueDate
    );
}
