using Microsoft.AspNetCore.Mvc;
using ServerLibrary.Repositories.Contracts;
using Shared.Dtos;

namespace Server.Controllers;

[ApiController]
[Route("/api/authentication")]
public class AuthenticationController(IUserAccount accountRepository) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto user)
    {
        var result = await accountRepository.CreateAsync(user);

        return Ok(result);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto user)
    {
        var result = await accountRepository.SignInAsync(user);

        return Ok(result);
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(RefreshTokenDto token)
    {
        var result = await accountRepository.RefreshJwtTokenAsync(token);

        return Ok(result);
    }
}