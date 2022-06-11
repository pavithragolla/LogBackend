using System.Text.Json.Serialization;
using LogBackend.Models;

namespace LogBackend.DTOs;

public record LogDTO
{
    [JsonPropertyName("id")]

    public long Id { get; set; }
    [JsonPropertyName("title")]


    public string Title { get; set; }
    [JsonPropertyName("description")]

    public string Description { get; set; }
    [JsonPropertyName("stack_trace")]

    public string StackTrace { get; set; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }
    [JsonPropertyName("updated_by_user_id")]
    public int UpdatedByUserId { get; set; }
    [JsonPropertyName("partially_deleted")]
    public bool PartiallyDeleted { get; set; }
    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; }
    // public List<TagTy> Tags { get; set; }
}


public record LogCreateDTO
{
    [JsonPropertyName("title")]
    public string Title { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; }
    [JsonPropertyName("stack_trace")]
    public string StackTrace { get; set; }
}
public record LogUpdateDTO
{
    [JsonPropertyName("description")]
    public string Description { get; set; }
}