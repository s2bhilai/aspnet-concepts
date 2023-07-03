using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using CarvedRock.WebApp;
using Serilog;
using Serilog.Exceptions;
using Serilog.Enrichers.Span;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, loggerConfig) => { 
    loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.WithProperty("Application",Assembly.GetExecutingAssembly().GetName().Name ?? "UI")
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.With<ActivityEnricher>()
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.Console()
    .WriteTo.Debug();
});

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";    
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
{
    options.Authority = "https://demo.duendesoftware.com";
    options.ClientId = "interactive.confidential";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
    options.Scope.Add("api");
    options.Scope.Add("offline_access");
    options.GetClaimsFromUserInfoEndpoint = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "email"
    };    
    options.SaveTokens = true;
});
//builder.Services.AddHttpContextAccessor();

builder.Services.AddOpenIdConnectAccessTokenManagement();

builder.Services.AddRazorPages();
//Using Named Client, so that common configuration can be grouped together
//builder.Services.AddHttpClient("backend", client =>
//{
//    client.BaseAddress = new Uri("https://localhost:7213");
//});

// To auto Renew access Token
builder.Services.AddUserAccessTokenHttpClient("backend", configureClient:  client =>
{
    client.BaseAddress = new Uri("https://localhost:7213");
});

var app = builder.Build();

app.UseExceptionHandler("/Error");

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseMiddleware<UserScopeMiddleware>();
app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapRazorPages().RequireAuthorization();

app.Run();
