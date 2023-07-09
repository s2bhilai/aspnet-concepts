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


builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

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

app.UseResponseCompression();
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

//Load Testing:
// NBomber for .NET Apis
// JMeter - heavy weight framework - UIs with cookie based auth.
// Use release builds
// Dont do this against production.
// Distributed tests are possible.

//HTTP/2
// - Header compression
// - Multiplex connections.
// - Faster load times

//HTTP/3
// - Faster connection setup
// - Better transition between networks

//HTTP/2 by default with asp net core 2.2

//Response Compression
// Relies on Accept-Encoding HTTP Header - gzip,br are common ones.
// Applicable to text based resources - html,css,js,json,xml
// Avoid further compression of already-compressed content - png,jpg
// Middleware available with in framework.

// Web servers like IIS,nginx, apache can do response compression often better so no need to use
// asp net core response compression, If application running not on these servers like kestrel use asp net core compression.

//Minification and Bundling
//Minification
// shrink css and javascript 
// Take only required parts
// Eliminates whitespace and comments
// Renames local vars
//Bundling
// Merging 2 or more files into one.
// Reduces number of requests.

// Memory management
// - .NET is a managed framework.
// - CLR manages memory and performs garbage collection.
// - Garbage Collection (GC)
// - Generations (0,1,2)
// - Roots
// - GC Modes
// - Large Object heap

// - Using too much memory slows performance.
// - Growing (leaking) memory usage willl eventually kill or recycle your app.
// Dependency Injection service lifetime
// Singleton - they should not have property or fields that keep on growing as singletons runs the entire lifetime of application.
// Scoped and transient items should free resources or used pooled ones.

//Statics should not grow heavily.

//Large Objects
// - Read large files or responses into memory.
// - Large arrays or lists.

//Garbage Collection manages memory allocation and release
// Gen 0 : Short lived objects, GC happens most often here
// Gen 1 : Survived Gen 0 collection, GC here if Gen 0 didnt reclaim enough
// Gen 2 : Singletons,statics, survived Gen 1 collection; GC here is slower.
// Root : Something that keeps an object alive (will not be GC'd).

//Collectively, 3 generations are called the Managed Heap, When application starts CLR will allocate  managed heap which will give our application virtual address space to work with. 
//GC is a completely blocking operation ( but is fast ).

//Roots prevent GC from freeing up memory.
// App pool recycles, app restarts (kubernetes) often point to leaks or incorrect limits.

//Analyzing memory usage
// VS supports memory usage analysis.
// Analysis often involves comparing "snapshots".
// dotnet-cli tools available to capture data - dotnet-counters
// Often perform analysis in conjuction with load test.