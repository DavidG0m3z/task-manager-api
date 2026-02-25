using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace TaskManager.Infrastructure.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		// Construir configuración apuntando al appsettings.json de Api
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../TaskManager.Api"))
			.AddJsonFile("appsettings.json", optional: false)
			.AddJsonFile("appsettings.Development.json", optional: true)
			.Build();

		// Obtener connection string
		var connectionString = configuration.GetConnectionString("DefaultConnection");

		// Crear DbContextOptions
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseSqlServer(connectionString);

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}