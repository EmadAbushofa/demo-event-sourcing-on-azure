using Grpc.Core;
using Grpc.Core.Interceptors;
using Todo.Command.Exceptions;

namespace Todo.Command.GrpcServices.Interceptors
{
    public class ApplicationExceptionInterceptor : Interceptor
    {
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (AlreadyExistsException exception)
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, exception.Message));
            }
        }
    }
}
