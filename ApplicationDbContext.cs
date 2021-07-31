using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace hotchocolate_playground
{
    /// MODELS AND CONTEXT CONFIGURATION
    public class Example
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public CompositeKey ComposedKey { get => new CompositeKey(Id, TypeId); }

    }

    public record CompositeKey(int Id, int TypeId);

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Example> Examples { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Example>()
                .HasKey(o => new { o.Id, o.TypeId });

            modelBuilder.Entity<Example>()
                .HasData(mocked);
        }

        public static List<Example> mocked => new List<Example>
        {
            new Example
            {
                Id = 1,
                TypeId = 1
            },
            new Example
            {
                Id = 1,
                TypeId = 2
            },
            new Example
            {
                Id = 1,
                TypeId = 3
            },
            new Example
            {
                Id = 1,
                TypeId = 4
            }
        };
    }
}