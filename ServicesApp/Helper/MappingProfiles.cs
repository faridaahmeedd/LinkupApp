using AutoMapper;
using ServicesApp.Core.Models;
using ServicesApp.Dto;
using ServicesApp.Dto.Authentication;
using ServicesApp.Models;

namespace ServicesApp.Helper
{
	public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<ServiceRequest, ServiceRequestDto>();
			CreateMap<ServiceRequestDto, ServiceRequest>();
			CreateMap<Customer, CustomerDto>();
			CreateMap<CustomerDto, Customer>();
			CreateMap<RegistrationDto, Customer>();
			CreateMap<CustomerDto, RegistrationDto>();
		}
    }
}
