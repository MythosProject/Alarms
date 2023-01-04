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

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
