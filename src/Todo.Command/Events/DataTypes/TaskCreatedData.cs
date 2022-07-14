namespace Todo.Command.Events.DataTypes
{
    public record TaskCreatedData(
        string Title,
        DateOnly DueDate,
        string Note
    );
}
