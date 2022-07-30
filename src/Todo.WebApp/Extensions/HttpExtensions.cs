using System.Net.Http.Json;
using Todo.WebApp.Models;

namespace Todo.WebApp.Extensions
{
    public static class HttpExtensions
    {
        public static async Task<ResponseResult<TResult>> GetAsync<TResult>(this HttpClient client, string url)
        {
            try
            {
                var response = await client.GetAsync(url);
                return await FetchResultAsync<TResult>(response);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return NotAvailable<TResult>();
            }
        }

        public static async Task<ResponseResult<TResult>> PostAsync<TInput, TResult>(this HttpClient client, string url, TInput input)
        {
            try
            {
                var response = await client.PostAsJsonAsync(url, input);
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
