using actionfilters.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//1. First way of using filters
//builder.Services.AddControllersWithViews(config =>
//{
    //This will attach with every action in every controller and every page request
    //config.Filters.Add(typeof(SimpleActionFilters));
//});

//2. Second way of using filters, then use ServiceFilter attribute on Action methods 
builder.Services.AddScoped<SimpleActionFilters>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
