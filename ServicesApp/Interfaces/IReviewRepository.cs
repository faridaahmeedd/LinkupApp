using Microsoft.EntityFrameworkCore;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IReviewRepository
	{
		ICollection<Review> GetReviews();
		ICollection<Review> GetReviewsOfRequest(int requestId);
		ICollection<Review> GetReviewsOfCustomer(string customerId);
		ICollection<Review> GetReviewsOfProvider(string providerId);
		Review GetReview(int id);
		bool ReviewExist(int id);
		bool IsCustomerAlreadyReviewed(int requestId);
		bool IsProviderAlreadyReviewed(int requestId);
		Task<bool> CreateCustomerReview(Review review);
		Task<bool> CreateProviderReview(Review review);
		Task<double> CalculateAvgRating(string Id);
		Task<bool> Warning(string Id);
        bool Save();
	}
}
