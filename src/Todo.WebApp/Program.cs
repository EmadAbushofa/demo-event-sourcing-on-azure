using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Todo.WebApp;
using Todo.WebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddServerApiClient(builder.Configuration);
builder.Services.AddAuthenticationService(builder.Configuration);
builder.Services.AddGrpcWebClient(builder.Configuration);
builder.Services.AddMatBlazorAndToaster();
builder.Services.AddBlazorFluentValidation();
builder.Services.AddScoped<INotificationService, ToastNotificationService>();

await builder.Build().RunAsync();
