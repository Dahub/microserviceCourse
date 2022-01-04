using PlatformService.Models;

namespace PlatformService.Data;

public static class DatabasePreparation
{
    public static void Populate(IApplicationBuilder applicationBuilder)
    {
        using(var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
        }
    }

    private static void SeedData(AppDbContext? context)
    {
        if(context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }
        if(context.Platforms.Any())
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