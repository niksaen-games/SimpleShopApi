using SimpleShopApi.Models.Dto;
using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Responses
{
    public class AuthResponse:Success
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("user")]
        public UserDto User { get; set; }
    }
}
