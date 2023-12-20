using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
    public interface ITimeSlotsRepository
    {
        bool UpdateTimeSlot(TimeSlot updatedTimeSlot);
    }
}
