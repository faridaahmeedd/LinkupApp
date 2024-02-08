using ServicesApp.Dto.Service;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
    public interface ITimeSlotsRepository
    {
		bool TimeSlotExist(int id);
		TimeSlot GetTimeSlot(int id);
		ICollection<TimeSlot> GetTimeSlotsOfService(int ServiceId);
		bool AddTimeSlots(List<TimeSlot> timeSlots);
		bool UpdateTimeSlots(List<TimeSlot> newTimeSlots, int serviceId);
		bool UpdateToTime(int OfferId);
		bool DeleteTimeSlot(int id);
		bool Save();
		bool CheckConflict(ServiceOffer offer);
		TimeSlot ConvertToModel(TimeSlotDto timeSlotDto);
		TimeSlotDto ConvertToDto(TimeSlot timeSlot);
	}
}
