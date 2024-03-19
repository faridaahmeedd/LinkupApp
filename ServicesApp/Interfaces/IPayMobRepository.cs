using Microsoft.AspNetCore.Mvc;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
    public interface IPayMobRepository
    {
        Task<string> FirstStep(int ServiceId);
        Task<string> SecondStep(string token, int ServiceId);
        Task<string> ThirdStep(string token, int orderId, int ServiceId);
        Task<string> CardPayment(string token);
        Task<dynamic> PostDataAndGetResponse(string url, object data);
        Task<bool> Refund(int TransactionId);

	}
}
