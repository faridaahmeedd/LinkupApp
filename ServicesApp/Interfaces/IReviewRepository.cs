﻿using Microsoft.EntityFrameworkCore;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IReviewRepository
	{
		ICollection<Review> GetReviews();
		ICollection<Review> GetReviewsOfCustomer(string customerId);
		ICollection<Review> GetReviewsOfProvider(string providerId);
		Review GetReview(int id);
		bool ReviewExist(int id);
		bool CreateReview(Review review);
		decimal CalculateAvgRating(string Id);
		bool Save();
	}
}
