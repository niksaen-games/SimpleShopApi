using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Requests
{
    public class ResetPasswordRequest
    {
        [JsonPropertyName("email")]
        [EmailAddress]
        public string Email { get; set; }

        [JsonPropertyName("confirmCode")]
        [MinLength(6)]
        [MaxLength(6)]
        public string Code { get; set; }

        [JsonPropertyName("password")]
        [MinLength(8)]
        [MaxLength(16)]
        public string Password { get; set; }
    }
}
