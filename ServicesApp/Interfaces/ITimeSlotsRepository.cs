using ServicesApp.Dto.Service;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
    public interface ITimeSlotsRepository
    {
		//ICollection<TimeSlot> GetTimeSlotsOfService();
		bool TimeSlotExist(int id);
		TimeSlot GetTimeSlot(int id);
		ICollection<TimeSlot> GetTimeSlotsOfService(int ServiceId);
		bool AddTimeSlot(List<TimeSlot> timeSlots);
		bool UpdateToTime(int OfferId);
		bool DeleteTimeSlot(int id);
		bool Save();
		bool CheckConflict(ServiceOffer offer);
	}
}
