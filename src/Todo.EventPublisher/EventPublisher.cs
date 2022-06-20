using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Todo.EventPublisher.Entities;
using Todo.EventPublisher.Services;

namespace Todo.EventPublisher
{
    public class EventPublisher
    {
        private readonly ServiceBusPublisher _publisher;

        public EventPublisher(ServiceBusPublisher publisher)
        {
            _publisher = publisher;
        }

        [FunctionName("EventPublisher")]
        [FixedDelayRetry(-1, "00:00:05")]
        public async Task Run([CosmosDBTrigger(
            databaseName: "%Database%",
            collectionName: "%Collection%",
            ConnectionStringSetting = "CosmosDbConnection",
            CreateLeaseCollectionIfNotExists = true,
            LeaseCollectionName = "%LeasesCollection%")]IReadOnlyList<Document> input,
            ILogger log)
        {
            try
            {
                log.LogInformation("New {Count} events to be published.", input.Count);

                foreach (var document in input)
                {
                    var @event = JsonConvert.DeserializeObject<Event>(document.ToString());

                    await _publisher.PublishAsync(@event);
                }
            }
            catch (Exception e)
            {
                log.LogError(e, "Document handler exception.");
                throw;
            }
        }
    }
}
