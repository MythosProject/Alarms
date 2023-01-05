using Mythos.Common.Authorization;
using Mythos.Common.Users;

namespace Alarms;

internal static class AlarmsApi
{
    public static RouteGroupBuilder MapAlarms(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/alarms");

        group.WithTags("Alarms");

        group.RequireAuthorization(pb => pb.RequireCurrentUser())
                     .AddOpenApiSecurityRequirement();

        group.MapGet("/", async (AlarmsDbContext db, CurrentUser owner) =>
        {
            var alarms = await db.Alarms.Where(alarm => alarm.UserId == owner.User!.Id).Select(t => t).AsNoTracking().ToListAsync();

            return new
            {
                count = alarms.Count,
                alarms,
            };
        });

        group.MapGet("/{id}", async Task<Results<Ok<Alarm>, NotFound>> (AlarmsDbContext db, int id, CurrentUser owner) =>
        {
            return await db.Alarms.FindAsync(id) switch
            {
                Alarm alarm when alarm.UserId == owner.User!.Id || owner.IsAdmin => TypedResults.Ok(alarm),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/test", async Task<Created<Alarm>> (AlarmsDbContext db, CurrentUser owner) =>
        {
            var alarm = new Alarm
            {
                Days = DaysOfTheWeek.Monday | DaysOfTheWeek.Friday,
                 Time = DateTimeOffset.Now,
                Name = "Take medicine",
                UserId = owner.User.Id,
            };

            return TypedResults.Created($"/alarms/{alarm.Id}", alarm);
        });

        group.MapPost("/", async Task<Created<Alarm>> (AlarmsDbContext db, AlarmDTO newAlarm, CurrentUser owner) =>
        {
            ArgumentNullException.ThrowIfNull(owner.User);

            var alarm = new Alarm
            {
                Name = newAlarm.Name,
                Days = newAlarm.Days,
                Time = newAlarm.Time,
                IsEnabled = true,
                UserId = owner.User?.Id,
            };

            db.Alarms.Add(alarm);
            await db.SaveChangesAsync();

            return TypedResults.Created($"/alarms/{alarm.Id}", alarm);
        });

        group.MapPut("/{id}", async Task<Results<Ok, NotFound, BadRequest>> (AlarmsDbContext db, int id, Alarm alarm, CurrentUser owner) =>
        {
            if (id != alarm.Id)
            {
                return TypedResults.BadRequest();
            }

            var rowsAffected = await db.Alarms
                .Where(t => t.Id == id && (t.UserId == owner.UserId || owner.IsAdmin))
                .ExecuteUpdateAsync(updates =>
                   updates.SetProperty(t => t.Name, alarm.Name));

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (AlarmsDbContext db, int id, CurrentUser owner) =>
        {
            var rowsAffected = await db.Alarms.Where(t => t.Id == id && (t.UserId == owner.UserId || owner.IsAdmin))
                                             .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }
}
