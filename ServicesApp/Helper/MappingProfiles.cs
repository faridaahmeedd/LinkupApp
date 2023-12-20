using AutoMapper;
using ServicesApp.Core.Models;
using ServicesApp.Dto.Authentication;
using ServicesApp.Dto.Service;
using ServicesApp.Dto.Users;
using ServicesApp.Models;

namespace ServicesApp.Helper
{
    public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<ServiceRequest, ServiceRequestDto>();
			CreateMap<ServiceRequestDto, ServiceRequest>();
			CreateMap<ServiceOffer, ServiceOfferDto>();
			CreateMap<ServiceOfferDto, ServiceOffer>();
			CreateMap<Customer, CustomerDto>();
			CreateMap<CustomerDto, Customer>();
			CreateMap<RegistrationDto, Customer>();
			CreateMap<CustomerDto, RegistrationDto>();
			CreateMap<Provider, ProviderDto>();
			CreateMap<ProviderDto, Provider>();
			CreateMap<RegistrationDto, Provider>();
			CreateMap<ProviderDto, RegistrationDto>();
            CreateMap<TimeSlot, TimeSlotDto>();
            CreateMap<TimeSlotDto, TimeSlot>();

        }
    }
}
