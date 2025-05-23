using Common.Domain.Entities;
using LogsService.Domain.Entiites;
using LogsService.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace LogsService.Infrastructure.Persistence;

public class ActionLogsDbContext(DbContextOptions<ActionLogsDbContext> options) : DbContext(options)
{
    public DbSet<ActionLog> ActionLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("action_logs");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ActionLogConfiguration).Assembly);
    }
}