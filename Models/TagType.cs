using LogBackend.DTOs;

namespace LogBackend.Models;


public record TagType
{
    public int Id { get; set; }
    public string Name { get; set; }


    public TagTypeDTO asDto => new TagTypeDTO
    {
        Id = Id,
        Name = Name
    };
}