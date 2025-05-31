using Common.API.Authentication;
using Common.API.Extensions;
using Common.Application.Options;
using Common.Domain.Dtos;
using Common.Domain.Enums;
using Common.Domain.Models.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersService.Application.Features.Users.GetById;
using UsersService.Application.Features.Users.GetPagedList;
using UsersService.Application.Features.Users.SetAvatarImage;
using UsersService.Application.Features.Users.UpdatePassword;
using UsersService.Domain.Dtos;
using UsersService.Domain.Entities;

namespace UsersService.API.Controllers;

[Route("/api/users")]
[ApiController]
public class UsersController(IMediator mediator) : ControllerBase
{ 
    [HttpGet]
    [Authorize]
    public async Task<PagedList<User>> GetPagedUsersListAsync(
        [FromQuery] UsersFilter usersFilter,
        [FromQuery] SortParams sortParams,
        [FromQuery] PageParams pageParams,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedUsersListQuery(
            usersFilter,
            sortParams,
            pageParams);
        
        return await mediator.Send(query, cancellationToken);
    }

    [HttpGet("{userId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetUserByIdQuery(userId), cancellationToken);
        return this.HandleResponse(response);
    }

    [Authorize]
    [HttpPatch("me/avatar")]
    public async Task<IActionResult> SetAvatarImageAsync(
        [FromForm] SetAvatarImageDto imageDto,
        CancellationToken cancellationToken)
    {
        var command = new SetAvatarImageCommand(User.GetId(), imageDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }

    [Authorize]
    [HttpPatch("me/password")]
    public async Task<IActionResult> SetNewPasswordAsync( 
        [FromBody] UpdatePasswordDto updatePasswordDto,
        CancellationToken cancellationToken)
    {
        var command = new UpdatePasswordCommand(User.GetId(), updatePasswordDto);
        var response = await mediator.Send(command, cancellationToken);
        return this.HandleResponse(response);
    }
    
    [Authorize]
    [HttpGet("get-claims")]
    public IActionResult GetUserClaimsData()
    {
        var userCookiesData = new UserCookieDataDto
        {
            UserId = User.FindFirst(CustomClaims.UserId)!.Value,
            NickName = User.FindFirst(CustomClaims.NickName)!.Value,
            AvatarImageName = User.FindFirst(CustomClaims.AvatarImageName)!.Value,
            Role = User.FindFirst(CustomClaims.Role)!.Value,
            Permissions = User.FindAll(CustomClaims.Permissions).Select(c => c.Value).ToList()
        };

        return Ok(new { userCookiesData });
    }
}