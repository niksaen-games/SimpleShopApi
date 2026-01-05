using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Requests
{
    public class UpdateUserRequest
    {
        [JsonPropertyName("name")]
        [RegularExpression(@"^[\p{L}&&[^0-9\s\p{P}\p{S}]]*$")]
        public string? Name { get; set; }

        [JsonPropertyName("surname")]
        [RegularExpression(@"^[\p{L}&&[^0-9\s\p{P}\p{S}]]*$")]
        public string? Surname { get; set; }
    }
}
