using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Requests
{
    public class ChangePasswordRequest
    {
        [JsonPropertyName("oldPassword")]
        [MinLength(8)]
        [MaxLength(16)]
        public string OldPassword { get; set; }

        [JsonPropertyName("newPassword")]
        [MinLength(8)]
        [MaxLength(16)]
        public string NewPassword { get; set; }
    }
}
