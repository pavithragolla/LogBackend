using LogBackend.DTOs;
using LogBackend.Models;
using Microsoft.AspNetCore.Mvc;

namespace logbackend.Controllers;

[ApiController]
[Route("api/tagtype")]


public class TagTypeContoller : ControllerBase
{
    private readonly ILogger<TagTypeContoller> _logger;
    private readonly ITagTypeRepository _tagType;

    public TagTypeContoller(ILogger<TagTypeContoller> logger, ITagTypeRepository tagType)
    {
        _logger = logger;
        _tagType = tagType;
    }

    [HttpGet]
    public async Task<ActionResult<List<TagType>>> GetAllTagTypes()
    {
        var allTodos = await _tagType.GetAllTagTypes();
        var dto = allTodos.Select(x => x.asDto);
        return Ok(dto);
    }
}