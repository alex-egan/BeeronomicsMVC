global using BeeronomicsMVC.Models;
global using BeeronomicsMVC.HostedServices;
global using BeeronomicsMVC.Services.DrinkService;
global using BeeronomicsMVC.Services.CrashService;
global using Microsoft.AspNetCore.SignalR;
global using BeeronomicsMVC.Hubs;
global using Microsoft.JSInterop;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("BeeronomicsDev");
builder.Services.AddDbContext<BeeronomicsDBContext>(x => x.UseSqlServer(connectionString));

builder.Services.AddSingleton<TimedHostedService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<TimedHostedService>());

//builder.Services.AddSingleton<IHostedService, Timers>(t => new Timers(new BeeronomicsDBContext()));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
}).AddNewtonsoftJsonProtocol();
builder.Services.AddScoped<IDrinkService, DrinkService>();
builder.Services.AddScoped<ICrashService, CrashService>();

builder.Services.AddMvc();

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapHub<DrinkHub>("/drinkHub");

app.Run();
