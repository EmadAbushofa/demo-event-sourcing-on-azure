namespace Todo.Query.EventHandlers.Created
{
    public record TaskCreatedData(
        string Title,
        DateTime DueDate,
        string Note
    );
}
