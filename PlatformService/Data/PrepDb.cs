using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd) {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            SeedData(scope.ServiceProvider.GetService<AppDbContext>(), isProd);
        }
    }

    private static void SeedData(AppDbContext? context, bool isProd) {
        if(context is null) {
            return;
        }

        if(isProd) {
            Console.WriteLine("QR501: Migrations...");
            try
            {
                context.Database.Migrate();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine($"QR501: Migration failed: {ex}");
            }
        }

        if(context.Platforms.Any()) {
            Console.WriteLine("QR501: Already have data");
            return;
        }

        Console.WriteLine("QR501: Seeding data...");

        context.Platforms.AddRange(
            Enumerable
                .Range(5, 4)
                .Select(i => new Platform {
                    Name = $".Net {i}.0",
                    Publisher = "Microsoft",
                    Cost = "Free"
                })
        );

        context.SaveChanges();
    }
}