using LogBackend.DTOs;

namespace LogBackend.Models;


public record Tag
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int TypeId { get; set; }
    public string TypeName { get; set; }
    // typename


    public TagDTO asDto => new TagDTO
    {
        Id = Id,
        Name = Name,
        CreatedAt = CreatedAt,
        TypeId = TypeId,
        TypeName = TypeName,
        // typename
    };
}