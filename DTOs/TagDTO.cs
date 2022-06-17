using System.Text.Json.Serialization;
using Logbackend.Models;

namespace LogBackend.DTOs;


public record TagDTO

{
    [JsonPropertyName("id")]

    public int Id { get; set; }
    [JsonPropertyName("name")]

    public string Name { get; set; }
    [JsonPropertyName("created_at")]

    public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("type_id")]

    public int TypeId { get; set; }
    [JsonPropertyName("type_name")]

    public string TypeName { get; set; }
    [JsonPropertyName("logs")]
    public List<LogDTO> Logs { get; set; }
    [JsonPropertyName("tagtypes")]

    public List<TagTypeDTO> TagTypes { get; set; }
}


public record TagCreateDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("type_id")]
    public int TypeId { get; set; }
}
public record TagUpdateDTO
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}