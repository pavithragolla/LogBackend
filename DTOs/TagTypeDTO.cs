using System.Text.Json.Serialization;

namespace LogBackend.DTOs;


public record TagTypeDTO
{
    [JsonPropertyName("id")]

    public int Id { get; set; }
    [JsonPropertyName("name")]

    public string Name { get; set; }
}
