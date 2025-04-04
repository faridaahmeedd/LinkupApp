﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServicesApp.Core.Models;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using System.Linq;
using System.Web.Helpers;

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
			return _context.Reviews.Include(p => p.Request).OrderBy(p => p.Id).ToList();
        }

		public ICollection<Review> GetReviewsOfRequest(int requestId)
		{
			return _context.Reviews.Where(r => r.Request.Id == requestId).OrderBy(p => p.Id).ToList();
		}

		public ICollection<Review> GetReviewsOfCustomer(string customerId)
		{
            var reviews = _context.Reviews
			   .Include(r => r.Request) 
			   .Where(r => r.Request.Customer.Id == customerId &&  r.ReviewerRole == "Provider" )
			   .OrderBy(r => r.Id)
			   .ToList();

            return reviews;
        }

		public ICollection<Review> GetReviewsOfProvider(string providerId)
		{

			var offers = _context.Offers.Include(o => o.Request).Where(o => o.Provider.Id == providerId).ToList();
            var requestIds = offers.Select(o => o.Request.Id).ToList();

            var reviews = _context.Reviews
                .Include(r => r.Request)
					.ThenInclude(req => req.Customer)
                .Where(r => requestIds.Contains(r.Request.Id) && r.ReviewerRole == "Customer")
                .OrderBy(r => r.Id)
                .ToList();

            return reviews;
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

		public bool CheckRequestOfReviewCompleted(int requestId)
		{
            var request = _context.Requests.FirstOrDefault(r => r.Id == requestId && r.Status == "Completed");
            if (request != null)
            {
				return true;
            }
            return false;
        }

        public bool Save()
		{
			var saved = _context.SaveChanges();
			return saved > 0 ? true : false;
		}

		public async Task<double> CalculateAvgRating(string Id)
		{
			var userReviews = GetReviewsOfProvider(Id);
            AppUser appUser = await _userManager.FindByIdAsync(Id);
            if (appUser != null)
			{
				var role = await _userManager.GetRolesAsync(appUser);
                if (role.Contains("Customer"))
				{
					userReviews = GetReviewsOfCustomer(Id);
				}
			}

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

		public async Task<bool> Warning(string Id)
		{

            var userReviews = GetReviewsOfProvider(Id);
            AppUser appUser = await _userManager.FindByIdAsync(Id);
            if (appUser != null)
            {
                var role = await _userManager.GetRolesAsync(appUser);
                if (role.Contains("Customer"))
                {
                    userReviews = GetReviewsOfCustomer(Id);
                }
            }
			double totalRating = userReviews.Sum(review => review.Rate ?? 0);
			int numberOfReviews = userReviews.Count();
			double avgRating = totalRating / numberOfReviews;

			if (numberOfReviews > 2)  
			{
				if (avgRating < 2.5)
				{
					_authRepository.SendMail(appUser.Email, "Warning", "Warning");
					return true;
				}
			}
			return false;
		}
	}
}