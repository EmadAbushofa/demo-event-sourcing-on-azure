using MatBlazor;

namespace Todo.WebApp.Services
{
    public static class MatBlazorExtensions
    {
        public static void AddMatBlazorAndToaster(this IServiceCollection services)
        {
            services.AddMatBlazor();
            services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.TopRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 100;
                config.VisibleStateDuration = 3000;
                config.ShowProgressBar = false;
            });
        }
    }
}
