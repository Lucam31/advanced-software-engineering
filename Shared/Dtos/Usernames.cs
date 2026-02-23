using System.Text.Json.Serialization;

namespace Shared.Dtos;

public class Usernames
{
    [JsonPropertyName("usernames")]
    public List<string> UsernamesList { get; set; } = new();
}