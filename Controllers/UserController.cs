using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LogBackend.DTOs;
using LogBackend.Models;
using LogBackend.Repositories;
using LogBackend.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace LogBackend.Controllers;


[ApiController]
[Route("api/user")]


public class UserController : ControllerBase
{

    private readonly ILogger<UserController> _logger;

    private readonly IUserRepository _user;
    private readonly ITagRepository _tag;

    private IConfiguration _config;

    public UserController(ILogger<UserController> logger, IUserRepository user, IConfiguration config, ITagRepository tag)
    {
        _logger = logger;
        _user = user;
        _config = config;
        _tag = tag;
    }
    private bool IsValidEmailAddress(string email)
    {
        try
        {
            var emailChecked = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch
        {
            return false;
        }
    }


    private string Generate(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
       {
            new Claim(UserConstants.Id, user.Id.ToString()),
            new Claim(UserConstants.Name, user.Name),
            new Claim(UserConstants.Email, user.Email),
            new Claim(UserConstants.IsSuperuser, user.IsSuperuser.ToString())
        };

        var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Audience"],
            claims,
            expires: DateTime.Now.AddMinutes(60),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);

    }

    [HttpPost]
    public async Task<ActionResult<UserDTO>> Create([FromBody] UserCreateDTo Data)
    {

        if (!IsValidEmailAddress(Data.Email))
            return BadRequest("Incorrect Email");

        var CreateUser = new User
        {
            Name = Data.Name,
            Email = Data.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(Data.Password),
            // CreatedAt = Data.CreatedAt,
            // Status = Data.Status,
            IsSuperuser = Data.IsSuperuser
        };
        var createdItem = await _user.Create(CreateUser);
        return Ok(createdItem);
    }


    // [HttpGet("{id}")]
    // public async Task<ActionResult<UserDTO>> GetUserById(int id)
    // {
    //     var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
    //     var userId = User.Claims.FirstOrDefault(c => c.Type == UserConstants.Id)?.Value;
    //     if (IsSuperuser.Trim().ToLower() == "true" || userId == id.ToString())
    //         return (await _user.GetUserById(id)).asDto;
    //     else
    //         return BadRequest("You are not authorized to access this resource");
    // }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserDTO>> GetUserById(int id)
    {
        // var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
        // var userId = User.Claims.FirstOrDefault(c => c.Type == UserConstants.Id)?.Value;
        // if (IsSuperuser.Trim().ToLower() == "true" || userId == id.ToString())
        //     return (await _user.GetUserById(id)).asDto;
        // else
        //     return BadRequest("You are not authorized to access this resource");

        var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;

        if (bool.Parse(IsSuperuser))
        {
            var res = await _user.GetUserById(id);

            if (res is null)
                return NotFound("No user found by id");

            var dto = res.asDto;
            dto.Tags = (await _user.GetTagUserById(id)).Select(x => x.asDto).ToList();
            return Ok(dto);
        }
        else
        {
            return BadRequest("You are not authorized to access this resource");
        }

    }

    [HttpPost("login")]
    public async Task<ActionResult<UserLoginResDTO>> Login(
        [FromBody] UserLoginDTO Data
    )
    {

        if (!IsValidEmailAddress(Data.Email))
            return BadRequest("Invalid email");

        var existingUser = await _user.GetByEmail(Data.Email);

        if (existingUser is null)
            return NotFound("User Not Found With Current Email Address");
        // if (existingUser.Password is null)
        // {
        //     return BadRequest(" Password is Null ");
        // }


        if (!BCrypt.Net.BCrypt.Verify(Data.Password, existingUser.Password))
            return BadRequest("Incorrect password");
        var token = Generate(existingUser);

        var res = new UserLoginResDTO
        {
            UserId = existingUser.Id,
            // Name = existingUser.Name,
            Email = existingUser.Email,
            Token = token,
            IsSuperuser = existingUser.IsSuperuser,
            DeviceId = Data.DeviceId
        };

        return Ok(res);
    }


    [HttpGet]
    [Authorize]

    public async Task<ActionResult<List<UserDTO>>> GetAllUser()
    {
        var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
        if (IsSuperuser.Trim().ToLower() != "true")
            return BadRequest("This is only for SuperUser");
        var AllUsers = await _user.GetAllUser();
        return Ok(AllUsers.Select(x => x.asDto));

    }


    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDTO>> Update([FromBody] UserUpdateDTO Data, int id)
    {
        var IsSuperuser = User.Claims.FirstOrDefault(c => c.Type == UserConstants.IsSuperuser)?.Value;
        if (IsSuperuser.Trim().ToLower() != "true")
            return BadRequest("This is only for SuperUser");
        var user = new User()
        {
            Id = id,
            Status = Data.Status,
        };
        var AllUsers = await _user.Update(user);
        return Ok(AllUsers);
    }



    private int GetuserIdFromClaims(IEnumerable<Claim> claims)
    {
        return Convert.ToInt32(claims.Where(x => x.Type == UserConstants.Id).First().Value);
    }
}

