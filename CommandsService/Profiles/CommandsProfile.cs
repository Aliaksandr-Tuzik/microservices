using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Command, CommandReadDto>();
        CreateMap<Platform, PlatformReadDto>();

        CreateMap<CommandCreateDto, Command>();

        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));

        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId));
    }
}