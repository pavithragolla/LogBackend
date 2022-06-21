using System.Security.Claims;
using Logbackend.Models;
using Logbackend.Repositories;
using LogBackend.DTOs;
using LogBackend.Repositories;
using LogBackend.Services;
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
    private IEmailService _email;



    public LogController(ILogger<LogController> logger, ILogRepository log, IConfiguration config, ITagRepository tag, IEmailService email)
    {
        _logger = logger;
        _log = log;
        _config = config;
        _tag = tag;
        _email = email;
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
        if (createdItem is null)
        {
            await _log.sendPushNotification();
            // _email.Send("Email Address");
        }
        return Ok(createdItem);
    }

    [HttpGet]
    public async Task<ActionResult<List<Log>>> GetAllLog([FromQuery] DateFilterDTO dateFilter = null, [FromQuery] LogFilterDTO logfilter = null)
    {
        // var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
        // if (IsSuperuser.Trim().ToLower() == "true")
        // {
        //     var AllLogs = (await _log.GetAllLog(dateFilter)).Select(x => x.asDto).ToList();
        //     return Ok(AllLogs);

        // }
        // if (IsSuperuser.Trim().ToLower() == "true")
        // {
        //     var AllLogs = (await _log.GetAllUserLog(dateFilter)).Select(x => x.asDto).ToList();
        //     return Ok(AllLogs);
        // }
        // return BadRequest("log not found");


        List<Log> allLog = new List<Log>();
        var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
        var userId = User.Claims.FirstOrDefault(c => c.Type == UserConstants.Id)?.Value;
        if (IsSuperuser.Trim().ToLower() == "true")
        {
            var AllLogs = (await _log.GetAllLog(dateFilter, logfilter)).Select(x => x.asDto).ToList();
            return Ok(AllLogs);

        }
        else
        {
            var AllLogs = (await _log.GetAllUserLog(dateFilter, logfilter)).Select(x => x.asDto).ToList();
            return Ok(AllLogs);
        }





    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Log>> GetById([FromRoute] int id)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == UserConstants.Id)?.Value;
        var Id = int.Parse(userId);

        var res = await _log.GetById(id);
        await _log.seenId(Id, res.Id);

        if (res is null)

            return NotFound("No Log Found with given id");
        await _log.seenId(Id, res.Id);
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
            UpdatedByUserId = Id,
        };

        var didUpdate = await _log.Update(toUpdateLog, Data.Tags);
        if (!didUpdate)
            return Ok("Log Updated");
        return Ok(didUpdate);
    }
    // [HttpPut("{id}/seen")]
    // public async Task<ActionResult> Seen([FromRoute] int id)
    // {
    //     var Id = GetuserIdFromClaims(User.Claims);
    //     var is_seen = await _log.GetById(id);
    //     var Update = res with
    //     {
    //         is_seen = true,
    //         UpdatedByUserId = Id,
    //     }
    //     if (is_seen is null)
    //     {

    //          await _log.unseen(Id, id);s
    //     }
    //     else
    //     {

    //     await _log.SetReadStatus(id, Id);
    //     }
    //     return Ok("Log seen Updated");
    // }

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
        // var didDelete = await _log.SoftDelete(id);
        if (!didDelete)
            return StatusCode(500, "Something went wrong");
        else
        {
            return Ok("Deleted");
        }
    }
    [HttpPut("softdelete/{id}")]
    public async Task<ActionResult> softDelete([FromRoute] long id)
    {

        var existingItem = await _log.GetById(id);
        if (existingItem.Id != existingItem.Id)
            return Unauthorized("You are not authorized to delete this comment");
        if (existingItem is null)
            return NotFound("Comment not found");
        var didDelete = await _log.SoftDelete(id);
        // var didDelete = await _log.SoftDelete(id);
        if (!didDelete)
            return StatusCode(500, "Something went wrong");
        else
        {
            return Ok("soft Deleted");
        }
    }
}