namespace Todo.WebApp.Models
{
    public class ResponseResult<TResult>
    {
        private readonly ProblemDetails? _problemDetails;
        private readonly TResult? _result;

        private ResponseResult(TResult? result, ProblemDetails? problemDetails)
        {
            _result = result;
            _problemDetails = problemDetails;
        }

        public static ResponseResult<TResult> Success(TResult? result) =>
            new(result, null);

        public static ResponseResult<TResult> Fail(ProblemDetails? problemDetails) =>
            new(default, problemDetails);

        public bool IsSuccess => _result != null;
        public ProblemDetails GetProblem() => _problemDetails ?? throw new InvalidOperationException("ProblemDetails is null");
        public TResult GetResult() => _result ?? throw new InvalidOperationException("Result is null");
    }
}
