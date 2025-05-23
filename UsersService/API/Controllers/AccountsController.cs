using Common.API.Extensions;
using Common.Application.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UsersService.Application.Features.Users.Login;
using UsersService.Application.Features.Users.Register;

namespace UsersService.API.Controllers;

[Route("/api/accounts")]
[ApiController]
public class AccountsController(
    IMediator mediator,
    IOptions<CustomCookieOptions> options) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterDto userRegisterDto)
    {
        var response = await mediator.Send(new RegisterUserCommand(userRegisterDto));
        return this.HandleResponse(response);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto userLoginDto)
    {
        var response = await mediator.Send(new LoginUserCommand(userLoginDto));
        
        if (response.IsFailure)
        {
            return this.HandleResponse(response);
        }

        var token = response.Data;
        HttpContext.Response.Cookies.Append(options.Value.Name, token);

        return Ok(new { token });
    }
    
    [Authorize]
    [HttpDelete("logout")]
    public IActionResult LogOut()
    {
        HttpContext.Response.Cookies.Delete(options.Value.Name);
        
        return Ok();
    }
}