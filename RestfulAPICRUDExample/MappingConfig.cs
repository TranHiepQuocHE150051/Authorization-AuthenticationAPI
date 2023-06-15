using AutoMapper;
using RestfulAPICRUDExample.Models;
using RestfulAPICRUDExample.Models.DTO;

namespace RestfulAPICRUDExample
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Villa, VillaDTO>();
            CreateMap<VillaDTO, Villa>();

            CreateMap<Villa, CreateVillaDTO>().ReverseMap();
            CreateMap<Villa, UpdateVillaDTO>().ReverseMap();
        }
    }
}
