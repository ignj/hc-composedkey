using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace hotchocolate_playground
{
    /// MODELS AND CONTEXT CONFIGURATION
    public class Example
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public string Id_TypeId { get => JsonSerializer.Serialize(new Id_TypeId(Id, TypeId)); }

        [InverseProperty(nameof(WrapperClass.Example))]
        public ICollection<WrapperClass> WraperClasses { get; set; }
    }

    public class WrapperClass
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public int ExampleId { get; set; }

        public Example Example { get; set; }
    }

    public record Id_TypeId(int Id, int TypeId);

    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Example> Examples { get; set; } = default!;
        public DbSet<WrapperClass> WrapperClasses { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Example>()
                .HasKey(o => new { o.Id, o.TypeId });

            modelBuilder.Entity<WrapperClass>()
                .HasOne(e => e.Example)
                .WithMany(e => e.WraperClasses)
                .HasForeignKey(k => new { k.ExampleId, k.TypeId });

            modelBuilder.Entity<Example>()
                .HasData(mocked);

            modelBuilder.Entity<WrapperClass>()
                .HasData(
                    new List<WrapperClass> 
                    { 
                        new WrapperClass 
                        { 
                            Id = 5,
                            ExampleId = 2,
                            TypeId = 4
                        }
                    });
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
                Id = 2,
                TypeId = 4
            }
        };
    }
}