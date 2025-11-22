using System.Diagnostics;
using System.Text.Json;
using GamesWithFriends.Application.Services;
using GamesWithFriends.Controllers.DTOs;
using GamesWithFriends.Controllers.Responses;
using GamesWithFriends.Core.Enums;
using GamesWithFriends.Core.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GamesWithFriends.Controllers.Common;

[ApiController]
[Route("/[controller]")]
public class AuthController(
    IAuthService authService,
    IOptions<AuthOptions> authOptions,
    IOptions<CookiePresetsOptions> cookiePresetsOptions) : ControllerBase
{
    private readonly AuthOptions _authOptionsValue = authOptions.Value;
    private readonly CookiePresetsOptions _cookiePresetsValue = cookiePresetsOptions.Value;
    
    [HttpPost("[action]")]
    public async Task<IActionResult> AuthenticateAsync(AuthenticateRequest request)
    {
        // Generating new access token
        var newAccessToken = await authService.GenerateTokenAsync(
            TokenType.Access,
            request.Username,
            request.Password);
        
        // Generating new refresh token
        var newRefreshToken = await authService.GenerateTokenAsync(
            TokenType.Refresh,
            request.Username,
            request.Password);

        // Saving refresh token to cookie and sending access token to client
        return Authenticate(newAccessToken, newRefreshToken);
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshAsync()
    {
        // Getting refresh token from cookies
        HttpContext.Request.Cookies.TryGetValue(
            _authOptionsValue.CookieKey, out var refreshToken);

        // If token is null, returning Unauthorized (401)
        if (string.IsNullOrWhiteSpace(refreshToken))
            return Unauthorized();
        
        // Getting new access and refresh tokens
        var (newAccessToken, newRefreshToken) = await authService.RefreshAsync(refreshToken);

        // Saving refresh token to cookie and sending access token to client
        return Authenticate(newAccessToken, newRefreshToken);
    }

    private IActionResult Authenticate(string? newAccessToken, string? newRefreshToken)
    {
        // Checking tokens
        if (string.IsNullOrWhiteSpace(newAccessToken) ||
            string.IsNullOrWhiteSpace(newRefreshToken))
            return BadRequest();

        // Getting cookie options for refresh token
        if(!_cookiePresetsValue.TryGetValue(CookiePresetType.SecureHttpOnly, out var cookiePreset))
            return StatusCode(500);
        
        // Saving refresh token to cookies
        HttpContext.Response.Cookies.Append(
            _authOptionsValue.CookieKey,
            newRefreshToken,
            cookiePreset);

        // Returning access token to client
        return Ok(new AuthenticateResponse(newAccessToken));
    }
}