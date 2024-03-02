using Microsoft.AspNetCore.Mvc;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IPayPalRepository
	{
		Task<string> CreatePayment(int ServiceId);
		Task<string> GetAccessToken();
		string GetApprovalLink(dynamic links);
		Task<dynamic> SendPayPalRequest(string endpoint, object requestData);
		Task<string> ExecutePayment(string paymentId, string token, string payerID);
	}
}
