using ExpanseTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpanseTracker.Infra.Persistence;

public class ExpanseTrackerDbContext : DbContext
{
    public ExpanseTrackerDbContext(DbContextOptions<ExpanseTrackerDbContext> optionsBuilder) : base(optionsBuilder)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<User>();

        builder.Entity<Expense>(x =>
        {
            x.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
            x.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);
        });

        builder.Entity<Category>();
    }
}
