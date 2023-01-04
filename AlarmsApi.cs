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
            return await db.Alarms.Where(alarm => alarm.OwnerId == owner.Id).Select(t => t).AsNoTracking().ToListAsync();
        });

        group.MapGet("/{id}", async Task<Results<Ok<Alarm>, NotFound>> (AlarmsDbContext db, int id, CurrentUser owner) =>
        {
            return await db.Alarms.FindAsync(id) switch
            {
                Alarm alarm when alarm.OwnerId == owner.Id || owner.IsAdmin => TypedResults.Ok(alarm),
                _ => TypedResults.NotFound()
            };
        });

        group.MapPost("/", async Task<Created<Alarm>> (AlarmsDbContext db, Alarm newAlarm, CurrentUser owner) =>
        {
            var alarm = new Alarm
            {
                Name = newAlarm.Name,
                OwnerId = owner.Id
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
                .Where(t => t.Id == id && (t.OwnerId == owner.Id || owner.IsAdmin))
                .ExecuteUpdateAsync(updates =>
                   updates.SetProperty(t => t.Name, alarm.Name));

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        group.MapDelete("/{id}", async Task<Results<NotFound, Ok>> (AlarmsDbContext db, int id, CurrentUser owner) =>
        {
            var rowsAffected = await db.Alarms.Where(t => t.Id == id && (t.OwnerId == owner.Id || owner.IsAdmin))
                                             .ExecuteDeleteAsync();

            return rowsAffected == 0 ? TypedResults.NotFound() : TypedResults.Ok();
        });

        return group;
    }
}
