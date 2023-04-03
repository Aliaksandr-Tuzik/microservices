using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient httpClient;
    private readonly IConfiguration configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        this.httpClient = httpClient;
        this.configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        var response = await httpClient.PostAsJsonAsync($"{configuration["CommandServiceUrl"]}/api/c/platforms", platform);

        if(response.IsSuccessStatusCode) {
            Console.WriteLine($"QR501: {nameof(SendPlatformToCommand)} - Success");
        } else {
            Console.WriteLine($"QR501: {nameof(SendPlatformToCommand)} - Failure: {response.StatusCode}");
        }
    }
}