using AggregatedAPI_TravelCompanion.DataTransferObjects.LocationDTO;
using AggregatedAPI_TravelCompanion.Models.LocationModels;
using AutoMapper;

namespace AggregatedAPI_TravelCompanion.Configurations;

public class MappingConfig
{
    public static MapperConfiguration RegisterMaps()
    {
        var mappingConfig = new MapperConfiguration(config =>
        {
            config.CreateMap<LocationResponse, LocationInfo>();
            config.CreateMap<LocationInfo, LocationResponse>();
        });
        return mappingConfig;
    }
}
