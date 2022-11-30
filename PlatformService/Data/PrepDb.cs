using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(WebApplicationBuilder builder)
    {
        using ServiceProvider provider = builder.Services.BuildServiceProvider();
        SeedData(provider.GetRequiredService<AppDbContext>(), builder.Environment);
    }

    private static void SeedData(AppDbContext context, IWebHostEnvironment env)
    {
        context.Database.Migrate();
        if (!context.Platforms.Any())
        {
            Console.WriteLine("--> Seeding Platforms");
            context.Platforms.AddRange(
                new Platform { Name = "Dotnet", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "Sql Server Express", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free" }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("--> Platforms has entities");
        }
    }
}
