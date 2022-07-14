namespace Todo.Command.Events.DataTypes
{
    public record TaskDueDateChangedData(
        DateOnly DueDate
    );
}
