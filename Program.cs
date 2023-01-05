using Alarms;

var builder = WebApplication.CreateBuilder(args);

builder.MythosBuilderStartup();

var connectionString = builder.Configuration.GetConnectionString("Alarms") ?? "Data Source=.db/Alarms.db";
builder.Services.AddSqlite<AlarmsDbContext>(connectionString, b => b.MigrationsAssembly("Alarms"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();
app.MapAlarms();
app.MythosAppStartup();

app.Map("/", () => Results.Redirect("/swagger"));

app.Run();
