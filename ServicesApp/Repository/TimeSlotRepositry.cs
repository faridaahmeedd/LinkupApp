using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
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

		public bool TimeSlotExist(int id)
		{
			return _context.TimeSlots.Any(p => p.Id == id);
		}

		public TimeSlot GetTimeSlot(int id)
		{
			return _context.TimeSlots.FirstOrDefault(p => p.Id == id);
		}

		public ICollection<TimeSlot> GetTimeSlotsOfService(int ServiceId)
		{
			return _context.TimeSlots.Where(p => p.ServiceRequest.Id == ServiceId).ToList();
		}

		public bool AddTimeSlot(TimeSlot timeSlot)
		{
			_context.Add(timeSlot);
			return Save();
		}

		public bool DeleteTimeSlot(int id)
		{
			_context.Remove(id);
			return Save();
		}

		//public bool UpdateTimeSlot(TimeSlot timeSlot)
        //{
        //  var existingTimeSlot = _context.TimeSlots.Find(timeSlot.Id);
		//	if (existingTimeSlot != null)
		//	{
		//		// Update properties of existingService with values from updatedService
		//		existingTimeSlot.FromTime = timeSlot.FromTime;
		//		existingTimeSlot.ToTime = timeSlot.ToTime;
		//		existingTimeSlot.Date = timeSlot.Date;
		//	}
		//  return Save();
		//}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}
	}
}
