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
                config.Position = MatToastPosition.BottomRight;
                config.PreventDuplicates = true;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 3000;
            });
        }
    }
}
