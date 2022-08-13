using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Todo.WebApp.Models;

namespace Todo.WebApp.Extensions
{
    public static class HttpExtensions
    {
        public static async Task<ResponseResult<TResult>> GetAsync<TResult>(this HttpClient client, string url, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await client.GetAsync(url, cancellationToken);
                return await FetchResultAsync<TResult>(response);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotAvailable<TResult>();
            }
        }

        public static async Task<ResponseResult<TResult>> PostAsync<TInput, TResult>(this HttpClient client, string url, TInput input, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await client.PostAsJsonAsync(url, input, cancellationToken);
                return await FetchResultAsync<TResult>(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotAvailable<TResult>();
            }
        }

        public static async Task<ResponseResult<TResult>> PutAsync<TInput, TResult>(this HttpClient client, string url, TInput input, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await client.PutAsJsonAsync(url, input, cancellationToken);
                return await FetchResultAsync<TResult>(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotAvailable<TResult>();
            }
        }

        public static async Task<ResponseResult<TResult>> PatchAsync<TInput, TResult>(this HttpClient client, string url, TInput input, CancellationToken cancellationToken = default)
        {
            try
            {
                var json = JsonSerializer.Serialize(input);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PatchAsync(url, content, cancellationToken);
                return await FetchResultAsync<TResult>(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotAvailable<TResult>();
            }
        }

        private static async Task<ResponseResult<TResult>> FetchResultAsync<TResult>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TResult>();
                return ResponseResult<TResult>.Success(result);
            }

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();

            if (problemDetails == null)
                return ResponseResult<TResult>.Fail(new ProblemDetails()
                {
                    Status = (int)response.StatusCode,
                    Detail = "Unkown error",
                    Title = "Unkown error",
                });

            return ResponseResult<TResult>.Fail(problemDetails);
        }

        private static ResponseResult<TResult> NotAvailable<TResult>()
        {
            return ResponseResult<TResult>.Fail(new ProblemDetails()
            {
                Detail = "Server connection failed.",
                Title = "Server connection failed.",
            });
        }
    }
}
