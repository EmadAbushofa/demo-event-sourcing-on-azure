namespace Todo.Command.Exceptions
{
    public class RuleViolationException : Exception
    {
        public RuleViolationException(string message) : base(message)
        {
        }
    }
}
