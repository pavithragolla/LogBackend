using Microsoft.AspNetCore.Mvc;

namespace LogBackend.DTOs;


public record TagFilterDTO
{
    private string _name = null;


    [FromQuery(Name = "name")]
    public string Name
    {
        get => _name;
        set => _name = value;
    }
}