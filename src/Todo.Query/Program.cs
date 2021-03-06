using MediatR;
using Serilog;
using Todo.Query.GrpcServices;
using Todo.Query.Services;
using Todo.Query.ServicesExtensions;

Log.Logger = LoggerServiceBuilder.Build();

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpcWithValidators();
builder.Services.AddMediatR(typeof(Program));
builder.Services.AddEntityFramework(builder.Configuration);
builder.Services.AddServiceBus();
builder.Services.AddHostedServices();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<TasksService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

public partial class Program { }