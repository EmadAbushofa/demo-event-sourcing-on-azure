﻿using Serilog;

namespace Todo.Command.Services
{
    public class LoggerServiceBuilder
    {
        public static Serilog.ILogger Build()
        {
            var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

            var serilogConfiguration = configuration.GetSection("Serilog");
            var appName = serilogConfiguration["AppName"];

            var logger = new LoggerConfiguration()
                .Enrich.WithProperty("name", appName)
                .ReadFrom.Configuration(configuration);

            return logger.CreateLogger();
        }
    }
}
