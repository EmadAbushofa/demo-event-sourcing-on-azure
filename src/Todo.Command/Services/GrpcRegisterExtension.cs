using Calzolari.Grpc.AspNetCore.Validation;
using Todo.Command.Validators;

namespace Todo.Command.Services
{
    public static class GrpcRegisterExtension
    {
        public static void AddGrpcWithValidators(this IServiceCollection services)
        {
            services.AddGrpc(o => o.EnableMessageValidation());

            AddValidators(services);
        }


        private static void AddValidators(IServiceCollection services)
        {
            services.AddGrpcValidation();
            services.AddValidator<CreateRequestValidator>();
        }
    }
}
