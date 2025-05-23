using Common.Domain.Entities;
using Common.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EmailService.Infrastructure.Persistence;

public class EmailConfirmationDbContext(
    DbContextOptions<EmailConfirmationDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("email_confirmation");
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
    }
}