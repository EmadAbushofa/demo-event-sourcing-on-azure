namespace Todo.Command.Services
{
    public class CosmosDbOptions
    {
        public const string CosmosDb = "CosmosDb";
        public string AccountEndpoint { get; set; } = string.Empty;
        public string AuthKey { get; set; } = string.Empty;
        public string DatabaseId { get; set; } = string.Empty;
        public string ContainerId { get; set; } = string.Empty;
        public int Throughput { get; set; }
    }
}
