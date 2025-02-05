using Microsoft.AspNetCore.Mvc;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using IdentityRefreshToken.Data;
using IdentityRefreshToken.Models;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using Duende.IdentityModel;
using static Duende.IdentityServer.Events.TokenIssuedSuccessEvent;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IClientStore _clientStore;
    private readonly HouseDbContext _context;
    private const string SecretKey = "secret"; // Store securely!
    private readonly byte[] _key = Encoding.UTF8.GetBytes(SecretKey);

    public TokenController(ITokenService tokenService, IClientStore clientStore, HouseDbContext context)
    {
        _tokenService = tokenService;
        _clientStore = clientStore;
        _context = context;
    }
    //[HttpPost("generate")]
    //public async Task<IActionResult> GenerateAccessToken([FromBody] TokenRequestModel model)
    //{
    //    // Validate the client
    //    var client = await _clientStore.FindClientByIdAsync(model.ClientId);
    //    if (client == null)
    //    {
    //        return BadRequest("Invalid client.");
    //    }

    //    // Create a token for the client
    //    var token = new Token
    //    {
    //        ClientId = model.ClientId,
    //        Issuer = "https://localhost:7059", // Update with your actual issuer
    //        //Lifetime = client.AccessTokenLifetime,
    //        Lifetime = 600,
    //        Claims = new List<System.Security.Claims.Claim>(),
    //        AccessTokenType = AccessTokenType.Jwt,
    //        CreationTime = DateTime.UtcNow,
    //        Audiences = new List<string> { "api1" },// Add your API scopes
    //    };

    //    // Generate the JWT
    //    var jwt = await _tokenService.CreateSecurityTokenAsync(token);

    //    return Ok(new { access_token = jwt, expires_in = token.Lifetime });
    //}

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateAccessToken([FromBody] AccessTokenRequest request)
    {
        // Validate the client
        var client = await _clientStore.FindClientByIdAsync("test-client");
        if (client == null)
        {
            return BadRequest("Invalid client.");
        }

        var user = _context.Users.SingleOrDefault(u => u.Username == request.Username && u.PasswordHash == request.PasswordHash);

        if (user != null)
        {
            // Create a token for the client
            var token = new Duende.IdentityServer.Models.Token
            {
                ClientId = "test-client",
                Issuer = "https://localhost:7059", // Update with your actual issuer
                                                   //Lifetime = client.AccessTokenLifetime,
                Lifetime = 40,
                Claims = new List<System.Security.Claims.Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id.ToString()),       // Unique user ID
                new Claim(JwtClaimTypes.Name, user.Username),    // Username
                new Claim(JwtClaimTypes.Role, "admin")          // User role (admin/user/etc.)
            },
                AccessTokenType = AccessTokenType.Jwt,
                CreationTime = DateTime.UtcNow,
                Audiences = new List<string> { "api1" },// Add your API scopes
            };

            // Generate the JWT
            var jwt = await _tokenService.CreateSecurityTokenAsync(token);

            var refreshToken = Guid.NewGuid();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);


            if (request.ClientType == "Web")
            {
                // Store refresh token in an HTTP-only cookie for web apps
                Response.Cookies.Append("refresh_token", refreshToken.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Use HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshTokenExpiry
                });

                return Ok(new AccessTokenResponse
                {
                    AccessToken = jwt,
                });
            }
            else if (request.ClientType == "Mobile")
            {
                // Send refresh token in response for mobile apps
                return Ok(new AccessTokenResponse
                {
                    AccessToken = jwt,
                    RefreshToken = refreshToken.ToString() // Mobile apps store securely
                });
            }
            else
            {
                return Ok(new AccessTokenResponse
                {
                    AccessToken = jwt,
                });
            }
        }
        else
        {
            return BadRequest("user not found or credential is wrong");
        }
    }
    

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel model)
    {
        if (model == null)
            return BadRequest(new { message = "Invalid client request" });

        var user = _context.Users.SingleOrDefault(u => u.RefreshToken == model.RefreshToken);

        if (user == null || user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token" });
        }
        AccessTokenRequest request = new AccessTokenRequest();
        request.Username = user.Username;
        request.PasswordHash = user.PasswordHash;

        // Await the access token generation if it's an async method
        var newAccessToken = await GenerateAccessToken(request); // Ensure this is awaited
        var newRefreshToken = GenerateRefreshToken();

        // Update user's refresh token and expiry time
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // Refresh Token expiry
        _context.SaveChanges();

        // Return both tokens
        //return Ok(new
        //{
        //    access_token = ((IdentityRefreshToken.Models.AccessTokenResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)newAccessToken).Value).AccessToken, // Return the string access token
        //    refresh_token = newRefreshToken // Return the new refresh token
        //});

        if (model.ClientType == "Web")
        {
            // Store refresh token in an HTTP-only cookie for web apps
            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Use HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = user.RefreshTokenExpiryTime
            });

            return Ok(new 
            {
                AccessToken = ((IdentityRefreshToken.Models.AccessTokenResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)newAccessToken).Value).AccessToken
            });
        }
        else if (model.ClientType == "Mobile")
        {
            // Send refresh token in response for mobile apps
            return Ok(new AccessTokenResponse
            {
                AccessToken = ((IdentityRefreshToken.Models.AccessTokenResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)newAccessToken).Value).AccessToken,
                RefreshToken = newRefreshToken.ToString() // Mobile apps store securely
            });
        }
        else
        {
            return Ok(new AccessTokenResponse
            {
                AccessToken = ((IdentityRefreshToken.Models.AccessTokenResponse)((Microsoft.AspNetCore.Mvc.ObjectResult)newAccessToken).Value).AccessToken,                
            });
        }
    }


}

public class TokenRequestModel
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
