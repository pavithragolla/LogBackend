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
    public DateTime CreatedAt { get; set; }
    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }
    [JsonPropertyName("updated_by_user_id")]
    public int UpdatedByUserId { get; set; }
    [JsonPropertyName("partially_deleted")]
    public bool PartiallyDeleted { get; set; }

    [JsonPropertyName("type_name")]
    public string TypeName { get; set; }
    // [JsonPropertyName("name")]
    // public string Name { get; set; }

    [JsonPropertyName("tags")]
    public List<TagDTO> Tags { get; set; }
    [JsonPropertyName("tagtypes")]

    public List<TagTypeDTO> TagTypes { get; set; }

    /////
    [JsonPropertyName("read_status")]
    public bool ReadStatus { get; set; }

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
    [JsonPropertyName("tags")]
    public List<int> Tags { get; set; }



    // [JsonPropertyName("name")]
    // public string Name { get; set; }
}
public record LogStatusUpdateDTO
{

    [JsonPropertyName("read_status")]
    public bool ReadStatus { get; set; }

}