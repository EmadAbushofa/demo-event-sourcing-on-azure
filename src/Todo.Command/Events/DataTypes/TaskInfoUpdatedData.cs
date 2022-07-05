namespace Todo.Command.Events.DataTypes
{
    public record TaskInfoUpdatedData(
        string Title,
        string Note
    );
}
