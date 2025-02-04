//using Duende.IdentityServer.Models;
//using IdentityRefreshToken.Data;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Security.Cryptography;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the DI container
//builder.Services.AddControllers(); // Register controllers
//builder.Services.AddEndpointsApiExplorer(); // Add this line for Swagger
//builder.Services.AddSwaggerGen(); // Add Swagger services
//builder.Services.AddDbContext<HouseDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddIdentityServer()
//    .AddInMemoryClients(new List<Client>
//    {
//        new Client
//        {
//            ClientId = "test-client",
//            AllowedGrantTypes = GrantTypes.ClientCredentials,
//            ClientSecrets = { new Secret("secret".Sha256()) },
//            AllowedScopes = { "api1" },
//            AccessTokenLifetime = 3600
//        }
//    })
//    .AddInMemoryApiScopes(new List<ApiScope>
//    {
//        new ApiScope("api1", "My API")
//    })
//    .AddSigningCredential(new SigningCredentials(new RsaSecurityKey(RSA.Create(2048)), SecurityAlgorithms.RsaSha256));

//var app = builder.Build();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger(); // Add Swagger middleware
//    app.UseSwaggerUI(); // Enable Swagger UI
//}

//app.UseIdentityServer();
//app.MapControllers();

//app.Run();


using Duende.IdentityServer.Models;
using IdentityRefreshToken.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container
builder.Services.AddEndpointsApiExplorer(); // Add this line for Swagger
builder.Services.AddSwaggerGen(); // Add Swagger services
builder.Services.AddDbContext<HouseDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentityServer(options =>
{
    options.IssuerUri = "https://localhost:7059"; // Matches the issuer in your token
})
.AddInMemoryClients(new List<Client>
{
    new Client
    {
        ClientId = "test-client", // Replace with your ClientId
        AllowedGrantTypes = GrantTypes.ClientCredentials, // Use Client Credentials grant
        ClientSecrets = { new Secret("secret".Sha256()) }, // Replace with your secret
        AllowedScopes = { "api1" }, // Match your API scope
        AccessTokenLifetime = 3600 // Token lifetime in seconds (optional)
    }
})
.AddInMemoryApiScopes(new List<ApiScope>
{
    new ApiScope("api1", "Your API Scope") // Matches the Audiences in your token
})
.AddDeveloperSigningCredential(); // Use for development only (creates a temporary signing key)

// 2. Configure JWT Bearer authentication for the API
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://localhost:7059"; // IdentityServer URL
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "api1", // Match the audience in your token
            ValidateIssuer = true,
            ValidIssuer = "https://localhost:7059", // Match the issuer in your token
            ValidateLifetime = true // Ensure token expiration is checked
        };
    });

// 3. Add authorization policies (optional, based on your requirements)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiScope", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireClaim("scope", "api1"); // Ensure the token has the "api1" scope
    });
});

builder.Services.AddControllers(); // Register controllers

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Add Swagger middleware
    app.UseSwaggerUI(); // Enable Swagger UI
}

app.MapControllers();

app.Run();

