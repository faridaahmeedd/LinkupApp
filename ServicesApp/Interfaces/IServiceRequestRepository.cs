﻿using ServicesApp.Dto;
using ServicesApp.Dto.Service;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IServiceRequestRepository
	{
		ICollection<ServiceRequest> GetServices();
		ServiceRequest GetService(int id);
		bool ServiceExist(int id);
		bool CreateService(ServiceRequest service);
		bool UpdateService(ServiceRequest service);
		bool DeleteService(int id);
		bool Save();
		ICollection<ServiceOffer> GetOffersOfService(int id);
		ServiceOffer AcceptedOffer(int serviceId);
        bool TimeSlotsExistInService(int ServiceId, int timeSlotId);
		bool CompleteService(int id);
		ICollection<ServiceDetailsDto> GetAllServicesDetails();

    }
}
