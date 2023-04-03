using AutoMapper;
using Grpc.Core;
using PlatformService.Data;

namespace PlatformService.SyncDataServices.Grpc;

public class GrpcPlatformService : GrpcPlatform.GrpcPlatformBase
{
    private readonly IPlatformRepo repository;
    private readonly IMapper mapper;

    public GrpcPlatformService(IPlatformRepo repository, IMapper mapper)
    {
        this.repository = repository;
        this.mapper = mapper;
    }

    public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
    {
        var response = new PlatformResponse();

        response.Platform.AddRange(
            repository.GetAllPlatforms().Select(p => mapper.Map<GrpcPlatformModel>(p))
        );

        return Task.FromResult(response);
    }
}