using Logbackend.Models;
using Logbackend.Repositories;
using LogBackend.DTOs;
using LogBackend.Repositories;
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


    [HttpPost]
    public async Task<ActionResult<Log>> Create([FromBody] LogCreateDTO Data)
    {

        var CreateLog = new Log
        {
            Title = Data.Title,
            Description = Data.Description,
            StackTrace = Data.StackTrace,
        };
        var createdItem = await _log.Create(CreateLog);
        return Ok(createdItem);
    }

    [HttpGet]
    public async Task<ActionResult<List<Log>>> GetAllLog([FromQuery] int Limit, int PageNumber)
    {
        var AllLogs = await _log.GetAllLog(Limit, PageNumber);

        return Ok(AllLogs);
    }

    [HttpGet("{id}")]

    public async Task<ActionResult> GetLogById(int id)
    {
        var allLogs = await _log.GetById(id);
        if (allLogs is null)
        {
            return NotFound("No logs found with given id");
        }
        var dto = allLogs.asDto;
        dto.Tags = await _tag.GetTagsByLogId(id);

        return Ok(dto);
    }

    // [HttpPut("{id}")]

    //   public async Task<ActionResult> Update([FromBody] LogUpdateDTO Data, int id)
    //   {
    //         var user = new Log
    //         {
    //             Id = id,
    //             Description = Data.Description
    //         };
    //         var AllUsers = await _log.Update(user);
    //         return Ok(AllUsers);
    //   }
      [HttpPut("{id}")]

    public async Task<ActionResult> Update([FromBody] LogUpdateDTO Data, int id)
    {

        var toUpdateLog = new Log
        {
            Description = Data.Description.Trim(),
            Id = id
        };

        var didUpdate = await _log.Update(toUpdateLog);
        return Ok(didUpdate);
    }

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