using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudyAPI.Services;

public interface ITokenServices
{
    JwtSecurityToken GereneteAccessToken(IEnumerable<Claim> claims, IConfiguration _config);//login

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token, IConfiguration _config);
}