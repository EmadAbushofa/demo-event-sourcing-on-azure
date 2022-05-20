using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Fluent;
using Todo.Command.Abstraction;
using Todo.Command.Infrastructure.Persistence;

namespace Todo.Command.Services
{
    public static class CosmosDbEventStreamExtension
    {
        public static void AddCosmosDbEventStream(this IServiceCollection services, IConfiguration configuration)
            => AddCosmosDbEventStreamAsync(services, configuration).GetAwaiter().GetResult();

        public static async Task AddCosmosDbEventStreamAsync(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetSection(CosmosDbOptions.CosmosDb).Get<CosmosDbOptions>();

            var client = new CosmosClientBuilder(options.AccountEndpoint, options.AuthKey)
                .WithSerializerOptions(new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase })
                .Build();

            var database = await client.CreateDatabaseIfNotExistsAsync(options.DatabaseId);

            var container = await database.Database.CreateContainerIfNotExistsAsync(
                id: options.ContainerId,
                partitionKeyPath: "/aggregateId",
                throughput: options.Throughput
            );

            services.AddSingleton(container.Container);
            services.AddTransient<IEventStore, CosmosDbEventStore>();
        }
    }
}
