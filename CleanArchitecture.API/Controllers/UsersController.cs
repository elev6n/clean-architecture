using CleanArchitecture.Application.Users.Commands.CreateUser;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    [HttpPost]
    public async Task<ActionResult<int>> CreateUser(CreateUserCommand command)
    {
        var result = await Mediator.Send(command);

        return HandleResult(result);
    }
}