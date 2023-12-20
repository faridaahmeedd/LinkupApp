using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
    public class TimeSlotRepositry  : ITimeSlotsRepository
    {
        private readonly DataContext _context;

        public TimeSlotRepositry(DataContext context)
        {
            this._context = context;
        }

        public bool UpdateTimeSlot(TimeSlot updatedTimeSlot)
        {
            var existingTimeSlot = _context.TimeSlots.Find(updatedTimeSlot.id);
            if (existingTimeSlot != null)
            {
                // Update properties of existingService with values from updatedService
                existingTimeSlot.FromTime = updatedTimeSlot.FromTime;
                existingTimeSlot.ToTime = updatedTimeSlot.ToTime;
                existingTimeSlot.Date = updatedTimeSlot.Date;

                _context.SaveChanges();
                return true;
            }

            return false;
        }
    }
}
