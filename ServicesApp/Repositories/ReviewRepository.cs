using Microsoft.AspNetCore.Identity;
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
		private readonly IAuthRepository _authRepository;
        private readonly UserManager<AppUser> _userManager;


        public ReviewRepository(DataContext context, IAuthRepository authRepository , UserManager<AppUser> userManager)
		{
			_context = context;
			_authRepository = authRepository;
			_userManager = userManager;

		}

		public ICollection<Review> GetReviews()
		{
			return _context.Reviews.Include(p => p.Customer).Include(p => p.Provider).OrderBy(p => p.Id).ToList();
		}

		public ICollection<Review> GetReviewsOfCustomer(string customerId)
		{
			return _context.Reviews.Include(p => p.Customer).Where(p => p.ReviewerRole =="Provider").OrderBy(p => p.Id).ToList();
		}

		public ICollection<Review> GetReviewsOfProvider(string providerId)
		{
			return _context.Reviews.Include(p => p.Provider).Where(p => p.ReviewerRole == "Customer").OrderBy(p => p.Id).ToList();
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

		public double CalculateAvgRating(string Id)
		{
			var reviews = GetReviews();
			var userReviews = reviews.Where(review => (review.ReviewerRole == "Customer" && review.Provider.Id == Id) 
											|| (review.ReviewerRole == "Provider" && review.Customer.Id == Id));


            if (userReviews.Any())
            {
                double totalRating = userReviews.Sum(review => review.Rate ?? 0);
                int numberOfReviews = userReviews.Count();

                if (numberOfReviews > 0)
                {
                    double avgRating = totalRating / numberOfReviews;
                    return Math.Round(avgRating, 2);
                }
				
            }
            return 0;
		}
		public async void Warning(string Id)
		{
			var reviews = GetReviews();
			var userReviews = reviews.Where(review => (review.ReviewerRole == "Customer" && review.Provider.Id == Id)
											|| (review.ReviewerRole == "Provider" && review.Customer.Id == Id));

			var user = await _userManager.FindByIdAsync(Id);
			double  totalRating = userReviews.Sum(review => review.Rate ?? 0);
			int numberOfReviews = userReviews.Count();
            double avgRating = totalRating / numberOfReviews;

            if (numberOfReviews > 2)  // 1 2 1 2 1
            {
                if (avgRating < 2.5)
                {
					Console.WriteLine("-------------------");
                    Console.WriteLine(user.Email);

                    _authRepository.SendMail(user.Email, "Warning", "Warning");
                }
            }
        }
	}
}
