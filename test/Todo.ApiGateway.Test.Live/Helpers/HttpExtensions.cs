using Microsoft.AspNetCore.Mvc;
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

        public static async Task<ValidationProblemDetails> GetErrorResult(this HttpResponseMessage message)
        {
            var body = await message.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
                throw new JsonException($"Body is null or empty with Status Code {message.StatusCode}.");

            var result = DeserializeJson<ValidationProblemDetails>(body);

            Assert.NotNull(result);

            return result!;
        }

        public static async Task<TResult> GetAsync<TResult>(this HttpClient client, string url)
        {
            var response = await client.GetAsync(url);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var result = DeserializeJson<TResult>(body);

            Assert.NotNull(result);

            return result!;
        }


        public static async Task<TResult> PostJsonAsync<TResult>(this HttpClient client, string url, object value)
        {
            var input = value.ToHttpContent();

            var response = await client.PostAsync(url, input);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var result = DeserializeJson<TResult>(body);

            Assert.NotNull(result);

            return result!;
        }

        public static async Task<TResult> PutJsonAsync<TResult>(this HttpClient client, string url, object value)
        {
            var input = value.ToHttpContent();

            var response = await client.PutAsync(url, input);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var result = DeserializeJson<TResult>(body);

            Assert.NotNull(result);

            return result!;
        }

        public static async Task<TResult> PatchJsonAsync<TResult>(this HttpClient client, string url, object? value = null)
        {
            var input = value?.ToHttpContent();

            var response = await client.PatchAsync(url, input);

            response.EnsureSuccessStatusCode();

            var body = await response.Content.ReadAsStringAsync();

            var result = DeserializeJson<TResult>(body);

            Assert.NotNull(result);

            return result!;
        }

        private static TValue? DeserializeJson<TValue>(string value)
            => JsonSerializer.Deserialize<TValue>(value, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = false,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });
    }
}
