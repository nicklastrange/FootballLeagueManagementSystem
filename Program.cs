using FootballLeagueManagementSystem.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = "Host=" + Environment.GetEnvironmentVariable("DATABASE_URL")
                               + ";Port=" + Environment.GetEnvironmentVariable("DATABASE_PORT")
                               + ";Database=" + Environment.GetEnvironmentVariable("DATABASE_NAME")
                               + ";Username=" + Environment.GetEnvironmentVariable("DATABASE_USERNAME")
                               + ";Password="+Environment.GetEnvironmentVariable("DATABASE_PASSWORD")+";";
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseNpgsql(connectionString)
);

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