using LogBackend.DTOs;

namespace LogBackend.Models;


public enum Status
{
    Active = 1,
    Deactive = 0,
}

public record User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
    public string Password { get; set; }
    public DateTimeOffset LastLogin { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public Status Status { get; set; }

    public bool IsSuperuser { get; set; }

    public UserDTO asDto => new UserDTO
    {
        Id = Id,
        Name = Name,
        Email = Email,
        LastLogin = LastLogin,
        CreatedAt = CreatedAt,
        Status = Status,
        IsSuperuser = IsSuperuser,

    };
}