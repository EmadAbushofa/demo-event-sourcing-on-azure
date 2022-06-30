using System.Text;
using System.Text.Json;

namespace Todo.ApiGateway.Test.Live.Helpers
{
    public static class HttpExtensions
    {
        public static HttpContent? ToHttpContent(this object value)
        {
            var json = JsonSerializer.Serialize(value);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }


        public static async Task<TResult> PostJsonAsync<TResult>(this HttpClient client, string url, object value)
        {
            var input = value.ToHttpContent();

            var response = await client.PostAsync(url, input);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<TResult>(body);

            Assert.NotNull(result);

            return result!;
        }
    }
}
