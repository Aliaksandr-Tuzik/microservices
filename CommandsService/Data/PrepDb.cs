using CommandsService.Models;
using CommandsService.SyncDataServices.Grpc;

namespace CommandsService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app) {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var grpcClient = scope.ServiceProvider.GetService<IPlatformDataClient>();

            if (grpcClient is null)
            {
                Console.WriteLine("QR501: Counldn't resolve gRPC client");
                return;
            }

            var platforms = grpcClient.ReturnAllPlatforms();

            var repository = scope.ServiceProvider.GetService<ICommandRepo>();

            if (repository is null)
            {
                Console.WriteLine("QR501: Counldn't resolve repository");
                return;
            }

            SeedData(repository, platforms);
        }
    }

    private static void SeedData(ICommandRepo repository, IEnumerable<Platform> platforms) {
        Console.WriteLine("QR501: Seeding platforms...");

        foreach (var platform in platforms)
        {
            if(repository.IsExternalPlatformExists(platform.ExternalId)) {
                continue;
            }

            repository.CreatePlatform(platform);
        }

        repository.SaveChanges();
    }
}