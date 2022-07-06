namespace Todo.Query.EventHandlers.InfoUpdated
{
    public record TaskInfoUpdatedData(
        string Title,
        string Note
    );
}
