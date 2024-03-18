using AutoMapper;
using ServicesApp.Core.Models;
using ServicesApp.Dto.Authentication;
using ServicesApp.Dto.Category;
using ServicesApp.Dto.Reviews_Reports;
using ServicesApp.Dto.Service;
using ServicesApp.Dto.Subcategory;
using ServicesApp.Dto.User;
using ServicesApp.Models;

namespace ServicesApp.Helper
{
    public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<ServiceRequest, PostServiceRequestDto>();
			CreateMap<PostServiceRequestDto, ServiceRequest>();
            CreateMap<ServiceRequest, GetServiceRequestDto>();
            CreateMap<GetServiceRequestDto, ServiceRequest>();
            CreateMap<ServiceRequest, ServiceDetailsDto>();
            CreateMap<ServiceDetailsDto, ServiceRequest>();
			CreateMap<Customer, GetCustomerDto>();
			CreateMap<GetCustomerDto, Customer>();
            CreateMap<Customer, PostCustomerDto>();
            CreateMap<PostCustomerDto, Customer>();
            CreateMap<RegistrationDto, Customer>();
			CreateMap<GetCustomerDto, RegistrationDto>();
			CreateMap<Provider, GetProviderDto>();
			CreateMap<GetProviderDto, Provider>();
            CreateMap<Provider, PostProviderDto>();
            CreateMap<PostProviderDto, Provider>();
            CreateMap<RegistrationDto, Provider>();
			CreateMap<GetProviderDto, RegistrationDto>();
			CreateMap<RegistrationDto, Admin>();
			CreateMap<Admin, RegistrationDto>();
            CreateMap<AppUser, RegistrationDto>();
            CreateMap<RegistrationDto, AppUser>();
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
			CreateMap<Subcategory, SubcategoryDto>();
			CreateMap<SubcategoryDto, Subcategory>();
            CreateMap<GetReviewDto, Review>();
            CreateMap<Review, GetReviewDto>();
            CreateMap<PostReviewDto, Review>();
            CreateMap<Review, PostReviewDto>();
			CreateMap<GetReportDto, Report>();
            CreateMap<Report, GetReportDto>();
            CreateMap<PostReportDto, Report>();
            CreateMap<Report, PostReportDto>();
            CreateMap<ServiceOffer, PostServiceOfferDto>()
				.ForMember(
					dest => dest.Duration,
					opt => opt.MapFrom(src => ConvertTimeToString(src.Duration))
				);
			CreateMap<PostServiceOfferDto, ServiceOffer>()
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
