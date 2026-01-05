using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Requests
{
    public class LoginRequest
    {
        [JsonPropertyName("email")]
        [EmailAddress]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        [MinLength(8)]
        [MaxLength(16)]
        public string Password { get; set; }
    }
}
