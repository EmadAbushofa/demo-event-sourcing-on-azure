namespace Todo.Query.EventHandlers.Created
{
    public record TaskCreatedData(
        string Title,
        DateOnly DueDate,
        string Note
    );
}
