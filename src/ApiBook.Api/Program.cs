using System.Text;
using ApiBook.Infrastructure;
using ApiBook.Logging;
using ApiBook.Presentation.Controllers;
using ApiBook.Api.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddStructuredLogging();
builder.Services.AddControllers()
    .AddApplicationPart(typeof(PlatformsController).Assembly);
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Security:Jwt:Issuer"] ?? throw new InvalidOperationException("Security:Jwt:Issuer missing.");
        var audience = builder.Configuration["Security:Jwt:Audience"] ?? throw new InvalidOperationException("Security:Jwt:Audience missing.");
        var signingKey = builder.Configuration["Security:Jwt:SigningKey"] ?? throw new InvalidOperationException("Security:Jwt:SigningKey missing.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStructuredRequestLogging();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();
