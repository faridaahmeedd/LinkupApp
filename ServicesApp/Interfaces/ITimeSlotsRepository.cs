using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
    public interface ITimeSlotsRepository
    {
		//ICollection<TimeSlot> GetTimeSlotsOfService();
		bool TimeSlotExist(int id);
		TimeSlot GetTimeSlot(int id);
		ICollection<TimeSlot> GetTimeSlotsOfService(int ServiceId);
		bool AddTimeSlot(TimeSlot timeSlot);
		//bool UpdateTimeSlot(TimeSlot timeSlot);
		bool DeleteTimeSlot(int id);
		bool Save();
	}
}
