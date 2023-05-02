using DAL;
using Implementations;
using Microsoft.EntityFrameworkCore.Design;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<IDesignTimeDbContextFactory<AppDbContext>, AppDbContextFactory>(x => 
    ActivatorUtilities.CreateInstance<AppDbContextFactory>(x, builder.Configuration.GetConnectionString(nameof(AppDbContext))));

builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 102400000;
});

builder.Services.AddHttpClient<BalancesApi>(client =>
    client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BalancesApiUrl")));

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapBlazorHub();

app.Run();