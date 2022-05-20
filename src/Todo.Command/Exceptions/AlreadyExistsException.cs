namespace Todo.Command.Exceptions
{
    public class AlreadyExistsException : Exception
    {
        public AlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
