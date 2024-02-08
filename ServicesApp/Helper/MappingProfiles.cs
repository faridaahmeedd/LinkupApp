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
            CreateMap<AppUser, RegistrationDto>();
            CreateMap<RegistrationDto, AppUser>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
			CreateMap<ServiceOffer, ServiceOfferDto>()
				.ForMember(
					dest => dest.Duration,
					opt => opt.MapFrom(src => ConvertTimeToString(src.Duration))
				);
			CreateMap<ServiceOfferDto, ServiceOffer>()
				.ForMember(
					dest => dest.Duration,
					opt => opt.MapFrom(src => ConvertStringToTime(src.Duration))
				);
			CreateMap<TimeSlot, TimeSlotDto>()
				.ForMember(
					dest => dest.Date,
					opt => opt.MapFrom(src => ConvertDateToString(src.Date))
				)
				.ForMember(
					dest => dest.FromTime,
					opt => opt.MapFrom(src => ConvertTimeToString(src.FromTime))
				);
			CreateMap<TimeSlotDto, TimeSlot>()
				.ForMember(
					dest => dest.Date,
					opt => opt.MapFrom(src => ConvertStringToDate(src.Date))
				)
				.ForMember(
					dest => dest.FromTime,
					opt => opt.MapFrom(src => ConvertStringToTime(src.FromTime))
				);
		}


		private TimeOnly ConvertStringToTime(string time)
		{
			DateTime result;
			return DateTime.TryParse(DateOnly.MinValue + " " + time, out result)
				? TimeOnly.FromDateTime(result)
				: TimeOnly.MinValue;
		}
		private DateOnly ConvertStringToDate(string date)
		{
			DateTime result;
			return DateTime.TryParse(date + " " + TimeOnly.MinValue, out result)
				? DateOnly.FromDateTime(result.Date)
				: DateOnly.MinValue;
		}

		private string ConvertTimeToString(TimeOnly time)
		{
			return time.ToString("HH:mm");
		}
		private string ConvertDateToString(DateOnly date)
		{
			return date.ToString("yyyy-M-d");
		}
	}
}
