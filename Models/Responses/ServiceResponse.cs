using System.Text.Json.Serialization;

namespace SimpleShopApi.Models.Responses
{
    public abstract class ServiceResponse
    {
        public static Success<T> Success<T>(T body) => new(body);
        public static Failed Failed(string message) => new(message);
    }

    public abstract class Success:ServiceResponse { }

    public sealed class Success<T>(T body): Success
    {
        public T Value { get; } = body;
    }

    public sealed class Failed(string message):ServiceResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; } = message;
    }
}
