using System.IdentityModel.Tokens.Jwt;
using CarvedRock.Data;
using CarvedRock.Domain;
using Hellang.Middleware.ProblemDetails;
using Microsoft.Data.Sqlite;
using CarvedRock.Api;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Host.UseSerilog((context, loggerConfig) => {
    loggerConfig
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.WithProperty("Application", Assembly.GetExecutingAssembly().GetName().Name ?? "API")
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .Enrich.With<ActivityEnricher>()
    .WriteTo.Seq("http://localhost:5341")
    .WriteTo.Console()
    .WriteTo.Debug();
});

builder.Services.AddProblemDetails(opts => 
{
    opts.IncludeExceptionDetails = (ctx, ex) => false;
    
    opts.OnBeforeWriteDetails = (ctx, dtls) => {
        if (dtls.Status == 500)
        {
            dtls.Detail = "An error occurred in our API. Use the trace id when contacting us.";
        }
    }; 
    opts.Rethrow<SqliteException>(); 
    opts.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
});

JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = "https://demo.duendesoftware.com";
        options.Audience = "api";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "email"
        };
    });

//In-memory caching
//1. Easy to consume resources on server.
//2. Cache Invalidation is hard (not distributed).


//Built in
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();



//Response Cache
//1. From the server side response, it will contain additional HTTP headers to cache the response data.
//2. Browser can avoid another HTTP request and use the content from cache.
//3. Must be HEAD or GET requests.
//4. Cannot have an Authorization Header.
//5. Not for server side UI apps - Razor Pages, MVC
//6. Usecases - Anonymous API calls, Static HTTP assets.
//7. cache-control: public,max-age=90
//8. Tag helpers - <cache> and <distributed-cache> can be used in Razor syntax.

//Output Caching - support in .NET7
//1. Must be GET or HEAD requests.
//2. Authenticated requests are not cached.
//3. Uses Memory Cache By Default
//   - Dont use IDistributedCache
//   - Can create custom IOutputCacheStore
// 4. Defaults can be overridden.
// 5. Caching at server side, For each http request, response is taken from cache at server side
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "CarvedRock";
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerOptions>();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IProductLogic, ProductLogic>();
builder.Services.AddDbContext<LocalContext>();
builder.Services.AddScoped<ICarvedRockRepository, CarvedRockRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<LocalContext>();
    context.MigrateAndCreateData();
}

app.UseMiddleware<CriticalExceptionMiddleware>();
app.UseProblemDetails();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.OAuthClientId("interactive.public.short");
        options.OAuthAppName("CarvedRock API");
        options.OAuthUsePkce();
    });
}

app.UseResponseCaching();
app.MapFallback(() => Results.Redirect("/swagger"));
app.UseAuthentication();
app.UseMiddleware<UserScopeMiddleware>();
app.UseSerilogRequestLogging();
app.UseAuthorization();
app.MapControllers().RequireAuthorization();

app.Run();


//Notes
//1. Asynchronous code enables more concurrency, not speed. 
//2. Improving concurrency improves performance under load, not a single transaction.

//3. Improper use of HttpContext can result in hangs,app crashes, or data corruption.
//4. HttpContext is not thread safe.
//5. HttpContext available in Controllers and Page Models.
//6. IHttpContextAccessor for other classes - Don't capture HttpContext.
//7. HttpContext is the built in property on the base PageModel and Controller class of AspNet Core, so don't inject in these cases.
//8. If a class is injecting IHttpContextAccessor dont assign the HttpContext property in a variable in class constructor. This may capture null or incorrect HttpContext.
//9. For forms and body content use async methods or framework features.

//10. Performance Tools
//11. Diagnostics: Measure your app, understand its performance. Find hot path.
//12. Benchmarking: Compare 2 approaches to see which is better.
//13. Load Testing: See how well your app performs under load conditions.
// Diagnostics - Request logging, Chart performace times - alert when slow, dotnet-trace tool.

// ILogger and StopWatch class can help.
// openTelemetry helps with microservices.
// Many other services - Application Insights, NewRelic, DataDog, Cloudwatch, Seq, Elastic Cloud.

//dotnet-trace ps
//dotnet-trace collect -p 32423 

//Benchmarking - create methods for different approaches.
//Run benchmarking - evaluate results - Release Build.
//Use optimized release builds.
//Match target processing environment (OS, CPU, language version etc).