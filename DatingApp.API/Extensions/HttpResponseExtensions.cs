using DatingApp.API.Helpers.Pagination;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace DatingApp.API.Extensions
{
    public static class HttpResponseExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationHeader paginationHeader)
        {
            var serializingOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string pgHeader = JsonSerializer.Serialize(paginationHeader, serializingOptions);

            response.Headers.Add("Pagination", pgHeader);
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }
}
