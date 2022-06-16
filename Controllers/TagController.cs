using Logbackend.Repositories;
using LogBackend.DTOs;
using LogBackend.Models;
using LogBackend.Repositories;
using LogBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace logbackend.Controllers;

[ApiController]
[Route("api/tag")]

public class TagController : ControllerBase
{
    private readonly ILogger<TagController> _logger;
    private readonly ITagRepository _tag;
    private readonly ILogRepository _log;

    public TagController(ILogger<TagController> logger, ITagRepository tag, ILogRepository log)
    {
        _logger = logger;
        _tag = tag;
        _log = log;
    }


    [HttpPost]
    [Authorize]

    public async Task<ActionResult<Tag>> Create([FromBody] TagCreateDTO Data)
    {
        var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
        if (IsSuperuser.Trim().ToLower() != "true")
            return BadRequest("This is only for SuperUser");
        var toCreateUser = new Tag
        {
            Name = Data.Name.Trim(),
            TypeId = Data.TypeId
        };
        var createdItem = (await _tag.Create(toCreateUser)).asDto;
        return Ok(createdItem);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Tag>> GetById([FromRoute] int id)
    {
        var tag = await _tag.GetById(id);
        var dto = tag.asDto;
        dto.Logs = (await _tag.GetTagsByLogId(id)).Select(x => x.asDto).ToList();
        dto.TagTypes = (await _tag.GetTagTypeByLogId(id)).ToList();
        return Ok(dto);
        // var dto = allTags;
        // dto.Log = await _log.GetById(Id);
    }


    [HttpGet]
    public async Task<ActionResult<List<Tag>>> GetAllTag([FromQuery] TagFilterDTO tagfilter = null)
    {
        var AllTags = await _tag.GetAllTags(tagfilter);
        var dto = AllTags.Select(x => x.asDto);
        return Ok(dto);
    }

    // [HttpDelete("{id}")]
    // // [Authorize]

    // public async Task<ActionResult> Delete([FromRoute] long id)
    // {
    //     // var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
    //     // if (IsSuperuser.Trim().ToLower() != "true")
    //     //     return BadRequest("This is only for SuperUser");
    //     var existingItem = await _tag.GetById(id);
    //     if (existingItem.Id != existingItem.Id)
    //         return Unauthorized("You are not authorized to delete this comment");
    //     if (existingItem is null)
    //         return NotFound("Comment not found");
    //     var didDelete = await _tag.DeleteTag(id);
    //     if (!didDelete)
    //         return BadRequest("Failed to delete comment");
    //     else
    //     {
    //         return Ok(didDelete);
    //     }
    // }

    // [HttpDelete("{id}")]
    // public async Task<ActionResult> DeleteTag([FromRoute] long id)
    // {

    //     var existingItem = await _tag.GetById(id);
    //     if (existingItem.Id != existingItem.Id)
    //         return Unauthorized("You are not authorized to delete this comment");
    //     if (existingItem is null)
    //         return NotFound("Comment not found");
    //     var didDelete = await _tag.DeleteTag(id);
    //     if (!didDelete)
    //         return StatusCode(500, "Something went wrong");
    //     else
    //     {
    //         return Ok("Deleted");
    //     }
    // }
}
