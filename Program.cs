using venueBooking.Data;
using venueBooking.Services;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure DbContext for MariaDB with EF Core logging
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
            connectionString,
            ServerVersion.AutoDetect(connectionString)
        )
        .EnableSensitiveDataLogging() // include parameter values in logs
        .LogTo(Console.WriteLine, LogLevel.Information) // log SQL and EF events
);

// 2. Register application services
builder.Services.AddSingleton<BlobService>();
builder.Services.AddSingleton<SearchService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 3. Apply any pending migrations and create DB/tables if needed
// Removed automatic migration to prevent errors if tables already exist.
// If you need to apply migrations, use the CLI: dotnet ef database update
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     db.Database.Migrate();
// }

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// 4. Default MVC route
// Remove automatic migration to prevent clearing or altering the database on every run
// If you need to apply migrations, do it manually using CLI or a separate admin tool

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();