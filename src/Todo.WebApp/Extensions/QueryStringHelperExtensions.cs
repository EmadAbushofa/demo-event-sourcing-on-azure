using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Todo.WebApp.Extensions
{
    public static class QueryStringHelperExtensions
    {
        public static TValue? GetValue<TValue>(this Dictionary<string, StringValues> queryStrings, string key)
        {
            if (queryStrings.TryGetValue(key, out var value))
            {
                return (TValue)Convert.ChangeType(value, typeof(TValue));
            }

            return default;
        }

        public static void SetFromQueryStrings<T>(this NavigationManager navigationManager, T filter)
        {
            var query = GetQueryOnly(navigationManager);
            var queryStrings = QueryHelpers.ParseQuery(query);

            foreach (var property in typeof(T).GetProperties())
            {
                if (queryStrings.TryGetValue(property.Name.ToLower(), out var valueString))
                {
                    var value = Parse(valueString, property.PropertyType);
                    if (value == null)
                    {
                        Console.WriteLine("Invalid type read for property " + property.Name);
                        continue;
                    }
                    property.SetValue(filter, value);
                }
            }
        }

        private static object? Parse(string value, Type type)
        {
            if (type.Equals(typeof(int)))
                return int.TryParse(value, out var number) ? number : null;

            if (type.Equals(typeof(bool)))
                return bool.TryParse(value, out var number) ? number : null;

            if (type.Equals(typeof(DateTime)) || type.Equals(typeof(DateTime?)))
                return DateTime.TryParse(value, out var date) ? date.Date : null;

            if (type.Equals(typeof(string)))
                return value;

            return null;
        }

        public static string ToQueryString<T>(this T query)
        {
            var listOfStrings = new HashSet<string>();

            foreach (var property in typeof(T).GetProperties())
            {
                var value = property.GetValue(query);

                if (value is DateTime date) value = date.ToString("yyyy-MM-dd");

                if (value != default)
                    listOfStrings.Add($"{property.Name.ToLower()}={value}");
            }

            return listOfStrings.Count == 0
                ? ""
                : '?' + string.Join('&', listOfStrings);
        }

        public static void SetUrlQueryString(this NavigationManager navigationManager, string query)
        {
            var urlWithoutQuery = GetUrlWithoutQuery(navigationManager);
            navigationManager.NavigateTo(urlWithoutQuery + query);
        }

        private static string GetUrlWithoutQuery(NavigationManager navigationManager) =>
            navigationManager.Uri.Split('?')[0];

        private static string? GetQueryOnly(NavigationManager navigationManager)
        {
            var parts = navigationManager.Uri.Split('?');
            return parts.Length > 1 ? parts[1].ToLower() : null;
        }
    }
}
