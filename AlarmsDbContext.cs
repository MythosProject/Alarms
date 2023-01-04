using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Mythos.Common.Users;

namespace Alarms;

public class AlarmsDbContext : DbContext
{
    public AlarmsDbContext(DbContextOptions<AlarmsDbContext> options)
        : base(options) { }

    public DbSet<Alarm> Alarms => Set<Alarm>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Alarm>();

        base.OnModelCreating(builder);
    }
}
