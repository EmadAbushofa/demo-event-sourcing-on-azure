namespace Todo.Query.Features.Create
{
    public record TaskCreatedData(
        string Title,
        DateTime DueDate,
        string Note
    );
}
