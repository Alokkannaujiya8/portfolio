using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Common.Interfaces;

namespace Portfolio.WebAPI.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ApiControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { Message = "Invalid credentials." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "PortfolioApp", "RefreshToken", refreshToken);

        return Ok(new AuthResponse(accessToken, refreshToken));
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return BadRequest(new { Message = "Invalid access token or refresh token." });
        }

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest(new { Message = "Invalid access token claims." });
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest(new { Message = "User not found." });
        }

        var savedRefreshToken = await _userManager.GetAuthenticationTokenAsync(user, "PortfolioApp", "RefreshToken");
        if (savedRefreshToken != request.RefreshToken)
        {
            return BadRequest(new { Message = "Invalid refresh token." });
        }

        var roles = await _userManager.GetRolesAsync(user);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var newAccessToken = _tokenService.GenerateAccessToken(claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        await _userManager.SetAuthenticationTokenAsync(user, "PortfolioApp", "RefreshToken", newRefreshToken);

        return Ok(new AuthResponse(newAccessToken, newRefreshToken));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user != null)
        {
            await _userManager.RemoveAuthenticationTokenAsync(user, "PortfolioApp", "RefreshToken");
        }

        return Ok(new { Message = "Logged out successfully." });
    }

    [Microsoft.AspNetCore.Authorization.Authorize]
    [HttpPost("change-credentials")]
    public async Task<IActionResult> ChangeCredentials([FromBody] ChangeCredentialsRequest request)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized();
        }

        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return BadRequest(new { Message = "User not found." });
        }

        // Verify current password
        if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
        {
            return BadRequest(new { Message = "Incorrect current password." });
        }

        // Update username / email if changed
        if (!string.IsNullOrEmpty(request.NewUsername) && request.NewUsername != user.Email)
        {
            var userExists = await _userManager.FindByEmailAsync(request.NewUsername);
            if (userExists != null)
            {
                return BadRequest(new { Message = "Username/Email already taken." });
            }

            user.Email = request.NewUsername;
            user.UserName = request.NewUsername;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new { Message = "Failed to update username." });
            }
        }

        // Update password if provided
        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            var changePassResult = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!changePassResult.Succeeded)
            {
                return BadRequest(new { Message = "Failed to update password. Password must satisfy complexity rules." });
            }
        }

        return Ok(new { Message = "Credentials updated successfully." });
    }
}

public record LoginRequest(string Email, string Password);
public record TokenRequest(string AccessToken, string RefreshToken);
public record AuthResponse(string AccessToken, string RefreshToken);
public record ChangeCredentialsRequest(string CurrentPassword, string NewUsername, string NewPassword);
