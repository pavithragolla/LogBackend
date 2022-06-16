using System.Security.Claims;
using Logbackend.Models;
using Logbackend.Repositories;
using LogBackend.DTOs;
using LogBackend.Repositories;
using LogBackend.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace LogBackend.Controllers;

[ApiController]
[Route("api/log")]

public class LogController : ControllerBase
{
    private readonly ILogger<LogController> _logger;
    private readonly ILogRepository _log;
    private readonly ITagRepository _tag;
    private IConfiguration _config;



    public LogController(ILogger<LogController> logger, ILogRepository log, IConfiguration config, ITagRepository tag)
    {
        _logger = logger;
        _log = log;
        _config = config;
        _tag = tag;
    }
    private int GetuserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }


    [HttpPost]
    public async Task<ActionResult<Log>> Create([FromBody] LogCreateDTO Data)
    {

        var CreateLog = new Log
        {
            Title = Data.Title,
            Description = Data.Description,
            StackTrace = Data.StackTrace,
        };
        var createdItem = (await _log.Create(CreateLog)).asDto;
        return Ok(createdItem);
    }

    [HttpGet]
    public async Task<ActionResult<List<Log>>> GetAllLog([FromQuery] DateFilterDTO dateFilter = null)
    {
        var AllLogs = (await _log.GetAllLog(dateFilter)).Select(x => x.asDto).ToList();

        return Ok(AllLogs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Log>> GetById([FromRoute] int id)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == UserConstants.Id)?.Value;
        var Id = int.Parse(userId);

        var res = await _log.GetById(id);
        // await _log.seenId(Id, res.Id);

        if (res is null)

            return NotFound("No Log Found with given id");
        //  await _log.seenId(Id, res.Id);
        var dto = res.asDto;
        dto.Tags = (await _log.GetTags(id)).Select(x => x.asDto).ToList();
        // dto.TagTypes = (await _log.GetLogTagTypesById(id)).Select(x => x.asDto).ToList();  // error at asDto
        dto.TagTypes = (await _log.GetLogTagTypesById(id)).ToList();

        return Ok(dto);
    }

    // [HttpGet("{id}")]
    // public async Task<ActionResult> GetLogById(int id)
    // {
    //     var allLogs = await _log.GetById(id);
    //     if (allLogs is null)
    //     {
    //         return NotFound("No logs found with given id");
    //     }
    //     var dto = allLogs.asDto;
    //     dto.Tags = await _tag.GetTagsByLogId(id);

    //     return Ok(dto);
    // }

    [HttpPut("{id}")]

    public async Task<ActionResult> Update([FromBody] LogUpdateDTO Data, [FromRoute] int id)
    {
        var Id = GetuserIdFromClaims(User.Claims);
        var res = await _log.GetById(id);
        if (res is null)
        {
            return NotFound("No logs found with given id");
        }
        var toUpdateLog = res with
        {
            Description = Data.Description.Trim(),
            // ReadStatus = Data.ReadStatus,
            UpdatedByUserId = Id,

        };

        var didUpdate = await _log.Update(toUpdateLog, Data.Tags);
        if (!didUpdate)
            return Ok("Log Updated");
        return Ok(didUpdate);
    }

    // [HttpPut("{id}/status")]

    // public async Task<ActionResult> UpdateSatus([FromBody] LogStatusUpdateDTO Data, [FromRoute] int id)
    // {
    //     var Id = GetuserIdFromClaims(User.Claims);
    //     var res = await _log.GetById(id);
    //     if (res is null)
    //     {
    //         return NotFound("No logs found with given id");
    //     }
    //     var toUpdateLog = res with
    //     {
    //         // Description = Data.Description.Trim(),
    //         ReadStatus = Data.ReadStatus,
    //         // UpdatedByUserId = Id,

    //     };

    //     var didUpdate = await _log.Update(toUpdateLog);
    //     return Ok(didUpdate);
    // }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete([FromRoute] long id)
    {

        var existingItem = await _log.GetById(id);
        if (existingItem.Id != existingItem.Id)
            return Unauthorized("You are not authorized to delete this comment");
        if (existingItem is null)
            return NotFound("Comment not found");
        var didDelete = await _log.DeleteLog(id);
        if (!didDelete)
            return StatusCode(500, "Something went wrong");
        else
        {
            return Ok("Deleted");
        }
    }
}