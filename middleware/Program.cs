var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();


app.UseWhen(context =>
{
    return (context.Request.Query.ContainsKey("cheese") && context.Request.Query["cheese"] == "cheddar");
}, innerApp =>
{

    innerApp.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("--> Im being used in a use when segment(1)\n");
        await next();
        await context.Response.WriteAsync("--> Im being used in a use when segment(1)\n");
    });

    //Next request in the pipeline will get executed.
});

app.MapWhen(context =>
{
    return context.Request.Query.ContainsKey("mapwhen");
}, innerApp =>
{
    innerApp.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("--> I'm being used in a map when segment (1)\n");
        await next();
        await context.Response.WriteAsync("--> I'm being used in a map when segment (1)\n");
    });

    //Here we have to use own Run(), otherwise default Run() will execute and will return error
    innerApp.Run(async context =>
    {
        await context.Response.WriteAsync("[STOP] I'm terminating inside a map when route\n");
    });
});


app.Map("/hello", innerApp =>
{
    innerApp.Use(async (context, next) =>
    {
        await context.Response.WriteAsync("--> Im being used in a mapped segment\n");
        await next();
        await context.Response.WriteAsync("<-- Im being used in a mapped segment\n");
    });

    innerApp.Run(async context =>
    {
        await context.Response.WriteAsync("[STOP] I'm terminating inside a mapped route\n");
    });
});

//If we dont have seperate/custom app.Run (only have default app.Run) after this app.Use,
//then there will be exception, since the below is setting 200 and app.run will try to set 404.
app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("--> Im being used....(1)\n");
    await next.Invoke(context);
    await context.Response.WriteAsync("<-- I'm returning from being used...(1)\n");
});

app.Use(async (HttpContext context, RequestDelegate next) =>
{
    await context.Response.WriteAsync("--> Im being used....(2)\n");
    await next.Invoke(context);
    await context.Response.WriteAsync("<-- I'm returning from being used...(2)\n");
});

app.Run(async context =>
{
    await context.Response.WriteAsync("Terminating Run \n");
});

app.Run(); //If there's only this app.Run() without any other middleware above this, then response will be 404
