using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using System.Linq;

namespace ServicesApp.Repositories
{
	public class ReviewRepository : IReviewRepository
	{
		private readonly DataContext _context;

		public ReviewRepository(DataContext context)
		{
			_context = context;
		}

		public ICollection<Review> GetReviews()
		{
			return _context.Reviews.Include(p => p.Customer).Include(p => p.Provider).OrderBy(p => p.Id).ToList();
		}

		public ICollection<Review> GetReviewsOfCustomer(string customerId)
		{
			return _context.Reviews.Include(p => p.Customer).Where(p => p.ReviewerRole =="Customer").OrderBy(p => p.Id).ToList();
		}

		public ICollection<Review> GetReviewsOfProvider(string providerId)
		{
			return _context.Reviews.Include(p => p.Provider).Where(p => p.ReviewerRole == "Provider").OrderBy(p => p.Id).ToList();
		}

		public Review GetReview(int id)
		{
			return _context.Reviews.Where(p => p.Id == id).FirstOrDefault();
		}

		public bool ReviewExist(int id)
		{
			return _context.Reviews.Any(p => p.Id == id);
		}

		public bool CreateReview(Review review)
		{
			_context.Add(review);
			return Save();
		}

		public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public decimal CalculateAvgRating(string Id)
		{
			var reviews = GetReviews();
			var userReviews = reviews.Where(review => (review.ReviewerRole == "Customer" && review.Provider.Id == Id) 
											|| (review.ReviewerRole == "Provider" && review.Customer.Id == Id));

			if (userReviews.Any())
			{
				decimal totalRating = userReviews.Sum(review => review.Rate ?? 0);
				int numberOfReviews = userReviews.Count();

				if (numberOfReviews > 0)
				{
					decimal avgRating = totalRating / numberOfReviews;
					return Math.Round(avgRating, 2);
				}
			}
			return 0;
		}
	}
}
