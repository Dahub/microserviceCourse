using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class DatabasePreparation
{
    public static void Populate(IApplicationBuilder applicationBuilder, bool isProduction)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProduction);
        }
    }

    private static void SeedData(AppDbContext? context, bool isProduction)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (isProduction)
        {
            Console.WriteLine("--> Attempting to apply migrations...");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex) 
            { 
                Console.WriteLine($"Could not apply migrations, reason : {ex.Message}");
            }
        }

        if (context.Platforms.Any())
        {
            Console.WriteLine("--> We already have data");
            return;
        }

        Console.WriteLine("--> Seeding data");

        context.Platforms.AddRange(
            new Platform()
            {
                Name = "DotNet",
                Publisher = "Microsoft",
                Cost = "Free"
            },
            new Platform()
            {
                Name = "Sql server express",
                Publisher = "Microsoft",
                Cost = "Free"
            },
            new Platform()
            {
                Name = "Kubernetes",
                Publisher = "Cloud Native Computing Foundation",
                Cost = "Free"
            }
        );

        context.SaveChanges();
    }
}