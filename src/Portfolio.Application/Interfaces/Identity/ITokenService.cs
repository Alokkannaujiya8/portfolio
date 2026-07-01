using System.Collections.Generic;
using System.Security.Claims;

namespace Portfolio.Application.Interfaces.Identity;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
