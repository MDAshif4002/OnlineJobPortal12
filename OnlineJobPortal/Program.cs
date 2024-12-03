using Microsoft.EntityFrameworkCore;
using OnlineJobPortal.Models;
using OnlineJobPortal.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Database Service 
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer( builder.Configuration.GetConnectionString("DefaultConnection") )
    );

// Add Session Service 
builder.Services.AddSession(
    options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes( 25 );
    }
 );

// Add Email Sender
builder.Services.AddSingleton<EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Website}/{action=Index}/{id?}");

app.Run();
