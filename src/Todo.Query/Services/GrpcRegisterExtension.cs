﻿using Calzolari.Grpc.AspNetCore.Validation;
using Todo.Query.GrpcServices.Interceptors;

namespace Todo.Query.Services
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
        }
    }
}
