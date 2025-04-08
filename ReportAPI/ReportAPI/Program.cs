using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

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
        x.RequireHttpsMetadata = false;
        // x.Authority = authServerUrl;
        // x.TokenValidationParameters.ValidIssuer = authServerUrl;
    });

services.AddAuthorization(o => o.AddPolicy("IsProthetic", b =>
{
    b.RequireRealmRoles("prothetic_user");
}));

services.AddKeycloakAuthorization(configuration);

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(config =>
{
    config.AllowAnyHeader();
    config.AllowAnyMethod();
    config.AllowAnyOrigin();
});

app.Logger.LogInformation(authServerUrl);
app.Logger.LogInformation(realm);
app.Logger.LogInformation(clientId);

app.MapControllers();
app.Run();