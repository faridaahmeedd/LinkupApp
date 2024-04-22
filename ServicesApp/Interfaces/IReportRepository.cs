using Microsoft.EntityFrameworkCore;
using ServicesApp.Models;

namespace ServicesApp.Interfaces
{
	public interface IReportRepository
	{
		ICollection<Report> GetReports();
		//ICollection<Report> GetReportsOfCustomer(string customerId);

		//ICollection<Report> GetReportsOfProvider(string providerId);

        Report GetReport(int id);

		bool ReportExist(int id);

		bool CreateReport(Report report);

		bool Save();
	}
}
