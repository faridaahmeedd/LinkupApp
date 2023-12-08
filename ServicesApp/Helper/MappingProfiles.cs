using AutoMapper;
using ServicesApp.Dto;
using ServicesApp.Models;

namespace ServicesApp.Helper
{
	public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<Service, ServiceDto>().PreserveReferences();
			CreateMap<ServiceDto, Service>();
		}
    }
}
