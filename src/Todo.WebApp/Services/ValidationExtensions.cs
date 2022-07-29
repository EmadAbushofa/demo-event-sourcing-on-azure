using Todo.WebApp.Validators;

namespace Todo.WebApp.Services
{
    public static class ValidationExtensions
    {
        public static void AddBlazorFluentValidation(this IServiceCollection services)
        {
            services.AddTransient<CreateViewModelValidator>();
        }
    }
}
