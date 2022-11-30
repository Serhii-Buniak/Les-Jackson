using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(WebApplicationBuilder builder)
    {
        using ServiceProvider provider = builder.Services.BuildServiceProvider();

        var grpcClient = provider.GetService<IPlatformDataClient>()!;

        IEnumerable<Platform>? plaforms = grpcClient.ReturnAllPlatforms();

        SeedData(provider.GetService<ICommandRepo>()!, plaforms);
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform>? platforms)
    {
        Console.WriteLine($"Seeding new Platforms");

        if (platforms is null)
        {
            return;
        }

        foreach (Platform platform in platforms)
        {
            if (!repo.ExternalPlatformExist(platform.ExternalId))
            {
                repo.CreatePlatform(platform);
            }
        }

        repo.SaveChanges();
    }
}