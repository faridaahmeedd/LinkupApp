using Microsoft.AspNetCore.Mvc;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
    public interface IPayMobRepository
    {
        Task<string> auth();
        Task<string> CardPayment(int ServiceId);
        Task<bool> Capture(int TransactionId, int ServiceId);
        Task<dynamic> PostDataAndGetResponse(string url, object data, string token);
    }
}
