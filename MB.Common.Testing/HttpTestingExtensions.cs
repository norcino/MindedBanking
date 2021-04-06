using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MB.Common.Testing
{
    public static class HttpTestingExtensions
    {
        public async static Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var responseString = await content.ReadAsStringAsync();

            if (typeof(T).IsGenericType &&
                (
                    typeof(T).GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                    typeof(T).GetGenericTypeDefinition() == typeof(ICollection<>) ||
                    typeof(T).GetGenericTypeDefinition() == typeof(List<>)
                ) &&
                typeof(T).GenericTypeArguments.Any())
            {
                if (!responseString.Contains("\"@odata"))
                    return JsonConvert.DeserializeObject<T>(responseString);

                // Get value from the content
                responseString = JObject.Parse(responseString)["value"].ToString();

                var collectionType = typeof(ICollection<>);
                var genericType = collectionType.MakeGenericType(typeof(T).GenericTypeArguments[0]);

                return (T)JsonConvert.DeserializeObject(responseString, genericType);
            }
            return JsonConvert.DeserializeObject<T>(responseString);
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient httpClient,
            string url, T entity)
        {
            var content = new StringContent(JsonConvert.SerializeObject(entity), Encoding.UTF8, "application/json");
            return await httpClient.PostAsync(url, content);
        }
    }
}
