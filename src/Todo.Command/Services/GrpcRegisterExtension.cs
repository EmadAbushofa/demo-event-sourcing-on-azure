using Calzolari.Grpc.AspNetCore.Validation;
using Todo.Command.Interceptors;
using Todo.Command.Validators;

namespace Todo.Command.Services
{
    public static class GrpcRegisterExtension
    {
        public static void AddGrpcWithValidators(this IServiceCollection services)
        {
            services.AddGrpc(options =>
            {
                options.EnableMessageValidation();
                options.Interceptors.Add<ApplicationExceptionInterceptor>();
            });

            AddValidators(services);
        }


        private static void AddValidators(IServiceCollection services)
        {
            services.AddGrpcValidation();
            services.AddValidator<CreateRequestValidator>();
            services.AddValidator<UpdateInfoRequestValidator>();
            services.AddValidator<ChangeDueDateRequestValidator>();
            services.AddValidator<CompleteRequestValidator>();
        }
    }
}
