using Microsoft.EntityFrameworkCore;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IReportRepository
	{
		ICollection<Report> GetReports();
		ICollection<Report> GetReportsOfRequest(int requestId);
		ICollection<Report> GetReportsOfCustomer(string customerId);
		ICollection<Report> GetReportsOfProvider(string providerId);
		Report GetReport(int id);
		bool ReportExist(int id);
		bool CreateCustomerReport(Report report);
		bool CreateProviderReport(Report report);
		bool Save();
	}
}
