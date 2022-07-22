using Calzolari.Grpc.AspNetCore.Validation;
using Todo.Query.Abstractions;
using Todo.Query.GrpcServices.Interceptors;
using Todo.Query.Services;
using Todo.Query.Validators;

namespace Todo.Query.ServicesExtensions
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

            AddNotificationsStream(services);
            AddValidators(services);
        }

        private static void AddValidators(IServiceCollection services)
        {
            services.AddGrpcValidation();
            services.AddValidator<FindRequestValidator>();
            services.AddValidator<SimilarTitleExistsRequestValidator>();
        }

        private static void AddNotificationsStream(IServiceCollection services)
        {
            services.AddSingleton<NotificationsStreamService>();
            services.AddSingleton<IMessagePublisher>(s => s.GetRequiredService<NotificationsStreamService>());
        }
    }
}
