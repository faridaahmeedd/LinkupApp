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
		bool UpdateToTime(TimeSlot timeSlot);
		bool DeleteTimeSlot(int id);
		bool Save();
	}
}
