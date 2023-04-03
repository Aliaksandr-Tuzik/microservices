using AutoMapper;
using CommandsService.Models;
using Grpc.Net.Client;

namespace CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration configuration;
    private readonly IMapper mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
        this.configuration = configuration;
        this.mapper = mapper;
    }

    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        Console.WriteLine($"QR501: Calling gRPC service: {configuration["GrpcPlatformUrl"]}");

        var channel = GrpcChannel.ForAddress(configuration["GrpcPlatformUrl"]);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();

        try
        {
            var response = client.GetAllPlatforms(request);

            return mapper.Map<IEnumerable<Platform>>(response.Platform);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"QR501: Failed to call gRPC server: {ex.Message}");

            return default;
        }
    }
}