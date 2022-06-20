using Microsoft.AspNetCore.Mvc;

namespace LogBackend.DTOs;


public record LogFilterDTO
{
    private string _title = null;


    [FromQuery(Name = "name")]
    public string Title
    {
        get => _title;
        set => _title = value;
    }
}