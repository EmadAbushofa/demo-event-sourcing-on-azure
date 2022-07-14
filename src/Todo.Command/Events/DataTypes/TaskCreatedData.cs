namespace Todo.Command.Events.DataTypes
{
    public record TaskCreatedData(
        string Title,
        DateTime DueDate,
        string Note
    );
}
