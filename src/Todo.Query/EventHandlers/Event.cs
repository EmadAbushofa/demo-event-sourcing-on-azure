using Newtonsoft.Json;

namespace Todo.Query.EventHandlers
{
    public record Event<T>(
        Guid AggregateId,
        int Sequence,
        string UserId,
        DateTime DateTime,
        T Data,
        int Version
    ) : IEvent
    {
        public string GetDataAsJson() =>
            JsonConvert.SerializeObject(Data, new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            });
    }
}
