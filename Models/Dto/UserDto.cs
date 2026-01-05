using SimpleShopApi.Models.Responses;
using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Dto
{
    public class UserDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("isEmailConfirmed")]
        public bool IsEmailConfirmed { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
