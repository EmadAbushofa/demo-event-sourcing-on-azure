using Calzolari.Grpc.Domain;
using Calzolari.Grpc.Net.Client.Validation;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Todo.ApiGateway.ActionFilters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order => int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception is RpcException rpcException)
            {
                var validationErrors = rpcException.GetValidationErrors().ToList();

                var modelState = GetErrors(validationErrors);

                context.Result = new ObjectResult(new ValidationProblemDetails(modelState)
                {
                    Title = validationErrors.FirstOrDefault()?.ErrorMessage,
                    Detail = rpcException.Status.Detail,
                    Type = rpcException.StatusCode.ToString(),
                    Status = rpcException.StatusCode switch
                    {
                        StatusCode.OK => StatusCodes.Status200OK,
                        StatusCode.Cancelled |
                        StatusCode.AlreadyExists |
                        StatusCode.OutOfRange |
                        StatusCode.FailedPrecondition => StatusCodes.Status409Conflict,
                        StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                        StatusCode.NotFound => StatusCodes.Status404NotFound,
                        StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                        StatusCode.Unauthenticated => StatusCodes.Status401Unauthorized,
                        _ => StatusCodes.Status500InternalServerError,
                    }
                });

                context.ExceptionHandled = true;
            }
        }

        private static ModelStateDictionary GetErrors(List<ValidationTrailers> validationErrors)
        {
            var modelState = new ModelStateDictionary();

            foreach (var error in validationErrors)
            {
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return modelState;
        }
    }
}
