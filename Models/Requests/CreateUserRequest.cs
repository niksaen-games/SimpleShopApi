using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Requests
{
    public class CreateUserRequest
    {
        [JsonPropertyName("name")]
        [RegularExpression(@"^[A-ZА-ЯЁ][a-zа-яё]*$")]
        public string Name { get; set; }

        [JsonPropertyName("surname")]
        [RegularExpression(@"^[A-ZА-ЯЁ][a-zа-яё]*$")]
        public string Surname { get; set; }

        [JsonPropertyName("email")]
        [EmailAddress]
        public string Email { get; set; }

        [JsonPropertyName("password")]
        [MinLength(8)]
        [MaxLength(16)]
        public string Password { get; set; }
    }
}
