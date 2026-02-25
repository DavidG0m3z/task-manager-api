using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Common;

namespace TaskManager.Infrastructure.Data;

public class ApplicationDbContext : DbContect
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    // DbSets (las tablas) -> como @Entity en JPA
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder)

        //Configuracion de Category
        modelBuilder.Entity<Category>(entity =>
        {

            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(500);

            entity.HashQueryFilter(e => !e.IsDeleted);

        });

        //Configuracion de TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.Totable("Tasks");
            entity.Haskey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Priority)
                .HasDefaultValue(1);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Task)
                .HasForeingKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);

        });

        seedData(modelBuilder);

        private void SeedData(ModelBuilder modelBuilder)
    {
        // Categorías iniciales
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Personal", Description = "Tareas personales", CreatedAt = DateTime.UtcNow },
            new Category { Id = 2, Name = "Trabajo", Description = "Tareas laborales", CreatedAt = DateTime.UtcNow },
            new Category { Id = 3, Name = "Estudio", Description = "Tareas de estudio", CreatedAt = DateTime.UtcNow }
        );

        // Tareas iniciales
        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem
            {
                Id = 1,
                Title = "Aprender Entity Framework Core",
                Description = "Completar tutorial de EF Core",
                CategoryId = 3,
                Priority = 3,
                DueDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Id = 2,
                Title = "Hacer ejercicio",
                Description = "Ir al gimnasio",
                CategoryId = 1,
                Priority = 2,
                DueDate = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    public override async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdateAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Soft Delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IdDelete = true;
                    entry.Entity.DelateAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangeAsync(cancellationToken);
    }
}
