using System.Text.Json.Serialization;
using LogBackend.Models;

namespace LogBackend.DTOs;


public record UserDTO
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    [JsonPropertyName("name")]

    public string Name { get; set; }
    [JsonPropertyName("email")]

    public string Email { get; set; }

    [JsonPropertyName("last_login")]

    public DateTimeOffset LastLogin { get; set; }
    [JsonPropertyName("created_at")]

    public DateTimeOffset CreatedAt { get; set; }
    [JsonPropertyName("status")]

    public Status Status { get; set; }

    [JsonPropertyName("is_superuser")]
    public bool IsSuperuser { get; set; }
    [JsonPropertyName("tags")]

    public List<TagDTO> Tags { get; set; }

}

public record UserCreateDTo
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("password")]
    public string Password { get; set; }
    //   [JsonPropertyName("status")]
    // // public DateTimeOffset CreatedAt { get; set; }
    // public Boolean Status { get; set; }

    [JsonPropertyName("is_superuser")]
    public bool IsSuperuser { get; set; }

}

public record UserLoginDTO
{
    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; }

}

public record UserLoginResDTO
{
    [JsonPropertyName("user_id")]
    public long UserId { get; set; }

    // [JsonPropertyName("name")]
    // public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }


    [JsonPropertyName("token")]
    public string Token { get; set; }
      [JsonPropertyName("is_superuser")]
    public bool IsSuperuser { get; set; }
    [JsonPropertyName("device_id")]
    public string DeviceId { get; set; }
}

public record UserUpdateDTO
{
    // [JsonPropertyName("name")]
    // public string Name { get; set; }
    // [JsonPropertyName("email")]
    // public string Email { get; set; }
    // [JsonPropertyName("id")]
    // public Status Id { get; set; }
    [JsonPropertyName("status")]
    public Status Status { get; set; }

}