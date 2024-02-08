using ServicesApp.Data;
using ServicesApp.Dto.Service;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Repository
{
    public class TimeSlotRepositry  : ITimeSlotsRepository
    {
        private readonly DataContext _context;
		private readonly IServiceOfferRepository _serviceOfferRepository;

		public TimeSlotRepositry(DataContext context, IServiceOfferRepository serviceOfferRepository)
        {
            _context = context;
			_serviceOfferRepository = serviceOfferRepository;
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

		public bool AddTimeSlots(List<TimeSlot> timeSlots)
		{
			foreach (var item in timeSlots)
			{
				_context.Add(item);
			}
			return Save();
		}

		public bool DeleteTimeSlot(int id)
		{
			_context.Remove(id);
			return Save();
		}

		public bool UpdateTimeSlots(List<TimeSlot> newTimeSlots, int serviceId)
		{
			ICollection<TimeSlot> existingTimeSlots = GetTimeSlotsOfService(serviceId);
			List<TimeSlot> timeSlotsToRemove = new List<TimeSlot>();

			foreach (var existingTimeSlot in existingTimeSlots.ToList())
			{
				var matchingNewTimeSlot = newTimeSlots.FirstOrDefault(newSlot =>
					newSlot.Date == existingTimeSlot.Date && newSlot.FromTime == existingTimeSlot.FromTime);

				if (matchingNewTimeSlot != null)
				{
					// Matching time slots remain as it is
					newTimeSlots.Remove(matchingNewTimeSlot); // Remove to avoid adding it again
				}
				else
				{
					// Time slot not found in the new list => update/delete it
					if (newTimeSlots.Count > 0)
					{
						var updatedTimeSlot = _context.TimeSlots.FirstOrDefault(p => p.Id == existingTimeSlot.Id);
						updatedTimeSlot.Date = newTimeSlots[0].Date;
						updatedTimeSlot.FromTime = newTimeSlots[0].FromTime;
						newTimeSlots.RemoveAt(0);
					}
					else
					{
						timeSlotsToRemove.Add(existingTimeSlot);
					}
				}
			}
			_context.RemoveRange(timeSlotsToRemove);
			_context.AddRange(newTimeSlots);

			return Save();
		}

		public bool UpdateToTime(int OfferId)
        {
			var offer = _context.Offers.FirstOrDefault(p => p.Id == OfferId);
			var existingTimeSlot = GetTimeSlot(offer.TimeSlotId);
			TimeOnly toTime = existingTimeSlot.FromTime.AddHours(offer.Duration.Hour);
			toTime = toTime.AddMinutes(offer.Duration.Minute);
			if (existingTimeSlot != null)
		    {
			    existingTimeSlot.ToTime = toTime;
				return Save();
			}
			return false;
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public bool CheckConflict(ServiceOffer offer)
		{
			var pendingOffers = _serviceOfferRepository.GetUnCompletedOffers(offer.Provider.Id);
			var newTimeSlot = _context.TimeSlots.Where(t =>  t.Id == offer.TimeSlotId).FirstOrDefault();
			TimeOnly toTime = newTimeSlot.FromTime.AddHours(offer.Duration.Hour);
			toTime = toTime.AddMinutes(offer.Duration.Minute);
			Console.WriteLine(toTime);
			if (pendingOffers != null)
			{
				foreach(var item in pendingOffers)
				{
					var offerTimeSlot = _context.TimeSlots.Where(t => t.Id == item.TimeSlotId).FirstOrDefault();
					if(newTimeSlot.Date == offerTimeSlot.Date)
					{
						if(newTimeSlot.FromTime > offerTimeSlot.ToTime)  //3 4     //6 6:30
						{
							return true;
						}
						if (toTime < offerTimeSlot.FromTime)  //5 9 old    //1 2
						{
							return true;
						}
						return false;
					}
				}
			}
			return true;
		}
	}
}
