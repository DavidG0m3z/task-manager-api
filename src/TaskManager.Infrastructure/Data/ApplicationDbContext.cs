using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Common;

namespace TaskManager.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    // DbSets (las tablas) -> como @Entity en JPA
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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

            entity.HasQueryFilter(e => !e.IsDeleted);

        });

        //Configuracion de TaskItem
        modelBuilder.Entity<TaskItem>(entity =>
        {
            entity.ToTable("Tasks");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(1000);

            entity.Property(e => e.Priority)
                .HasDefaultValue(1);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasQueryFilter(e => !e.IsDeleted);

        });

        //Configuracion de Role
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Role");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .HasMaxLength(200);

            entity.HasIndex(e => e.Name)
                .IsUnique();

            entity.HasQueryFilter(e => !e.IsDeleted);

        });

        //Configuracion de User
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasOne(e => e.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(e => e.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        SeedData(modelBuilder);
    }
        
    private void SeedData(ModelBuilder modelBuilder)
    {

        var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var futureDateShort = new DateTime(2024, 12, 15, 0, 0, 0, DateTimeKind.Utc);
        var futureDateLong = new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Utc);

        // Categorías iniciales
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Personal", Description = "Tareas personales", CreatedAt = seedDate },
            new Category { Id = 2, Name = "Trabajo", Description = "Tareas laborales", CreatedAt = seedDate },
            new Category { Id = 3, Name = "Estudio", Description = "Tareas de estudio", CreatedAt = seedDate }
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
                DueDate = futureDateLong,
                CreatedAt = seedDate
            },
            new TaskItem
            {
                Id = 2,
                Title = "Hacer ejercicio",
                Description = "Ir al gimnasio",
                CategoryId = 1,
                Priority = 2,
                DueDate = futureDateLong,
                CreatedAt = seedDate
            }
        );

        // Roles iniciales
        modelBuilder.Entity<Role>().HasData(
            new Role
            {
                Id = 1,
                Name = "Admin",
                Description = "Administrador con acceso completo",
                CreatedAt = seedDate,
                IsDeleted = false
            },
            new Role
            {
                Id = 2,
                Name = "User",
                Description = "Usuario estándar con acceso limitado",
                CreatedAt = seedDate,
                IsDeleted = false
            }
        );

        // Usuario Admin inicial
        // Contraseña: "Admin123!" (hasheada con BCrypt)
        // IMPORTANTE: En producción, cambiar este password inmediatamente
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Email = "admin@taskmanager.com",
                PasswordHash = "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy",  // Admin123!
                FirstName = "Admin",
                LastName = "User",
                IsActive = true,
                RoleId = 1,  // Admin role
                CreatedAt = seedDate,
                IsDeleted = false
            }
        );
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
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
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Deleted:
                    // Soft Delete
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    break;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
