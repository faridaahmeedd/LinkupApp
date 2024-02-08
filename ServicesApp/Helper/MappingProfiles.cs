using AutoMapper;
using ServicesApp.Core.Models;
using ServicesApp.Dto.Authentication;
using ServicesApp.Dto.Category;
using ServicesApp.Dto.Service;
using ServicesApp.Dto.Users;
using ServicesApp.Models;
using System.Globalization;

namespace ServicesApp.Helper
{
    public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<ServiceRequest, ServiceRequestDto>();
			CreateMap<ServiceRequestDto, ServiceRequest>();
            CreateMap<ServiceRequest, ServiceDetailsDto>();
            CreateMap<ServiceDetailsDto, ServiceRequest>();
			CreateMap<ServiceOffer, ServiceOfferDto>()
				.ForMember(
					dest => dest.Duration,
					opt => opt.MapFrom(src => src.Duration.ToString("HH:mm"))
				);
			CreateMap<ServiceOfferDto, ServiceOffer>()
				.ForMember(
					dest => dest.Duration,
					opt => opt.MapFrom(src => ConvertToTimeOnly(src.Duration))
				);
			CreateMap<Customer, CustomerDto>();
			CreateMap<CustomerDto, Customer>();
			CreateMap<RegistrationDto, Customer>();
			CreateMap<CustomerDto, RegistrationDto>();
			CreateMap<Provider, ProviderDto>();
			CreateMap<ProviderDto, Provider>();
			CreateMap<RegistrationDto, Provider>();
			CreateMap<ProviderDto, RegistrationDto>();
			CreateMap<RegistrationDto, Admin>();
			CreateMap<Admin, RegistrationDto>();
			CreateMap<TimeSlot, TimeSlotDto>();
            CreateMap<TimeSlotDto, TimeSlot>();
            CreateMap<AppUser, RegistrationDto>();
            CreateMap<RegistrationDto, AppUser>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
		}



		private TimeOnly ConvertToTimeOnly(string duration)
		{
			DateTime result;
			Console.WriteLine("ConvertToTimeOnly");
			return DateTime.TryParse(DateOnly.MinValue + " " + duration, out result)
				? TimeOnly.FromDateTime(result)
				: TimeOnly.MinValue;
		}

	}
}
