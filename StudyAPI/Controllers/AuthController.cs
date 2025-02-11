using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudyAPI.DTOs.Authentication;
using StudyAPI.Models;
using StudyAPI.Services;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace StudyAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenServices _tokenServices;
    private readonly UserManager<ApplicationUser> _userManeger;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenServices tokenServices, UserManager<ApplicationUser> userManeger, RoleManager<IdentityRole> roleManager, IConfiguration config, ILogger<AuthController> logger)
    {
        _tokenServices = tokenServices;
        _userManeger = userManeger;
        _roleManager = roleManager;
        _config = config;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginModel loginModel)
    {
        var user = await _userManeger.FindByNameAsync(loginModel.Username!);

        if (user is not null && await _userManeger.CheckPasswordAsync(user, loginModel.Password!))
        {
            var userRoles = await _userManeger.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id", user.UserName!), //custom claim para testar a politica de autorização.
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenServices.GereneteAccessToken(authClaims, _config); // retorno um token
            string refreshToken = _tokenServices.GenerateRefreshToken(); //retorno um refreshToken

            _ = int.TryParse(_config["JWT:RefreshTokenValidInMinutes"], out int refreshTokenValidInMinutes);

            user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidInMinutes); //atualizo o tempo de expiração do refreshToken na base de dados
            user.RefreshToken = refreshToken; //atualizo o refreshToken na base de dados para receber acesso sem credenciais usando o action RefreshToken

            await _userManeger.UpdateAsync(user);

            return Ok(new
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                Expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterModel registerModel)
    {
        var userExist = await _userManeger.FindByNameAsync(registerModel.Username!);
        if (userExist is not null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });
        }

        ApplicationUser user = new()
        {
            Email = registerModel.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = registerModel.Username
        };

        var result = await _userManeger.CreateAsync(user, registerModel.Password!); //realiza o hash da senha e persiste no banco o user.
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
        }

        return StatusCode(StatusCodes.Status201Created, new Response { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken(TokenModel tokenModel)
    {
        string? accessToken = tokenModel.AccesToken ?? throw new ArgumentNullException(nameof(accessToken));
        string? refreshToken = tokenModel.RefreshToken ?? throw new ArgumentNullException(nameof(refreshToken));

        var principal = _tokenServices.GetPrincipalFromExpiredToken(accessToken, _config);
        if (principal is null)
        {
            return BadRequest("Invalid access token");
        }

        string username = principal!.Identity!.Name!;

        var user = await _userManeger.FindByNameAsync(username);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now) //caso o refreshToken ja tenha expirado, retorne error; 
        {
            return BadRequest("Invalid refresh token");
        }

        var newAccessToken = _tokenServices.GereneteAccessToken(principal.Claims.ToList(), _config);
        string newRefreshToken = _tokenServices.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;

        await _userManeger.UpdateAsync(user);

        return new ObjectResult(new
        {
            accessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            refreshToken = newRefreshToken
        });
    }

    [HttpPost("revoketoken")]
    [Authorize(Policy = "ExclusiveOnly")]
    public async Task<ActionResult> RevokeToken(string username)
    {
        var user = await _userManeger.FindByNameAsync(username);
        if (user is null) return BadRequest("User not found");

        user.RefreshToken = null;
        await _userManeger.UpdateAsync(user);
        return NoContent();
    }

    [HttpPost("create-role")]
    [Authorize(Policy = "SuperAdminOnly")]
    public async Task<IActionResult> CreateRoles(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName)) return BadRequest("Role name is required");

        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (roleResult.Succeeded)
            {
                _logger.LogInformation($"Role {roleName} created successfully!");
                return StatusCode(StatusCodes.Status201Created,
                    new Response { Status = "Success", Message = $"Role {roleName} created successfully!" });
            }
            else
            {
                _logger.LogInformation("Role creation failed!");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = "Role creation failed! Please check role details and try again." });
            }
        }

        return StatusCode(StatusCodes.Status400BadRequest,
            new Response { Status = "Error", Message = "Role already exists!" });
    }

    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string email, string roleName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(roleName)) return BadRequest("Username and role name are required");

        var user = await _userManeger.FindByEmailAsync(email);
        if (user is null) return BadRequest("User not found");

        var role = await _roleManager.FindByNameAsync(roleName);

        if (role is null) return BadRequest("Role not found");

        var result = await _userManeger.AddToRoleAsync(user, role.Name!);
        
        if (result.Succeeded)
        {
            _logger.LogInformation($"Role {role.Name} assigned to user {user.UserName} successfully!");
            return StatusCode(StatusCodes.Status201Created, new Response { Status = "Success", Message = $"Role {role.Name} assigned to user {user.UserName} successfully!" });
        }
        else
        {
            _logger.LogInformation("Role assignment failed!");
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Role assignment failed! Please check role details and try again." });
        }
    }
}