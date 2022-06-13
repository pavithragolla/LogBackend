using LogBackend.DTOs;

namespace Logbackend.Models;


public record Log
{
    public long Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public string StackTrace { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; }
    public int UpdatedByUserId { get; set; }
    public bool PartiallyDeleted { get; set; }
    public string TypeName { get; set; }





    public LogDTO asDto => new LogDTO
    {
        Id = Id,
        Title = Title,
        Description = Description,
        StackTrace = StackTrace,
        UpdatedByUserId = UpdatedByUserId,
        TypeName = TypeName,

    };
}