using Microsoft.AspNetCore.Mvc;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IClientStore _clientStore;

    public TokenController(ITokenService tokenService, IClientStore clientStore)
    {
        _tokenService = tokenService;
        _clientStore = clientStore;
    }
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateToken([FromBody] TokenRequestModel model)
    {
        // Validate the client
        var client = await _clientStore.FindClientByIdAsync(model.ClientId);
        if (client == null)
        {
            return BadRequest("Invalid client.");
        }

        // Create a token for the client
        var token = new Token
        {
            ClientId = model.ClientId,
            Issuer = "https://localhost:7059", // Update with your actual issuer
            Lifetime = client.AccessTokenLifetime,
            Claims = new List<System.Security.Claims.Claim>(),
            AccessTokenType = AccessTokenType.Jwt,
            CreationTime = DateTime.UtcNow,
            Audiences = new List<string> { "api1" } // Add your API scopes
        };

        // Generate the JWT
        var jwt = await _tokenService.CreateSecurityTokenAsync(token);

        return Ok(new { access_token = jwt, expires_in = client.AccessTokenLifetime });
    }
}

public class TokenRequestModel
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}
