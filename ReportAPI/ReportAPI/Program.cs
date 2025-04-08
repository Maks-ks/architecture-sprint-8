using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.IdentityModel.Tokens;

var authServerUrl = Environment.GetEnvironmentVariable("WEB_API_KEYCLOAK_URL");
var realm = Environment.GetEnvironmentVariable("WEB_API_KEYCLOAK_REALM");
var clientId = Environment.GetEnvironmentVariable("WEB_API_KEYCLOAK_CLIENT_ID");

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;

configuration["Keycloak:auth-server-url"] = authServerUrl;
configuration["Keycloak:realm"] = realm;
configuration["Keycloak:resource"] = clientId;

services.AddKeycloakWebApiAuthentication(configuration, (x) =>
{
    x.Authority = $"{authServerUrl}/realms/{realm}";
    x.RequireHttpsMetadata = false;
    x.Audience = clientId;

    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

services.AddAuthorization(o => o.AddPolicy("IsProthetic", b =>
{
    b.RequireRealmRoles("prothetic_user");
}));

services.AddKeycloakAuthorization(configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("policy", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:3000"
                )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("policy");

app.Logger.LogInformation(authServerUrl);
app.Logger.LogInformation(realm);
app.Logger.LogInformation(clientId);

app.MapControllers();
app.Run();