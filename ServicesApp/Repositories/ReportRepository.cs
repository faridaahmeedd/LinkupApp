using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using System.Linq;

namespace ServicesApp.Repositories
{
	public class ReportRepository : IReportRepository
    {
        private readonly DataContext _context;
		private readonly IServiceRequestRepository _serviceRequestRepository;

		public ReportRepository(DataContext context, IServiceRequestRepository serviceRequestRepository)
        {
            _context = context;
			_serviceRequestRepository = serviceRequestRepository;
		}

        public ICollection<Report> GetReports()
        {
            return _context.Reports.Include(p => p.Request).OrderBy(p => p.Id).ToList();
        }

		public ICollection<Report> GetReportsOfRequest(int requestId)
		{
			return _context.Reports.Where(r => r.Request.Id == requestId).OrderBy(p => p.Id).ToList();
		}

		public ICollection<Report> GetReportsOfCustomer(string customerId)
        {
            return _context.Reports
               .Include(r => r.Request)
               .Where(r => r.Request.Customer.Id == customerId && r.ReporterRole == "Provider")
               .OrderBy(r => r.Id)
               .ToList();
        }

        public ICollection<Report> GetReportsOfProvider(string providerId)
        {
            var offers = _context.Offers.Include(o => o.Request).Where(o => o.Provider.Id == providerId).ToList();

            var requestIds = offers.Select(o => o.Request.Id).ToList();

            var reports = _context.Reports
                .Include(r => r.Request)
                    .ThenInclude(req => req.Customer)
                .Where(r => requestIds.Contains(r.Request.Id) && r.ReporterRole == "Customer")
                .OrderBy(r => r.Id)
                .ToList();

            return reports;
        }

        public Report GetReport(int id)
        {
            return _context.Reports.Include(p => p.Request).Where(p => p.Id == id).FirstOrDefault();
        }

        public bool ReportExist(int id)
        {
            return _context.Reports.Any(p => p.Id == id);
        }

		public bool CreateCustomerReport(Report report)
		{
			var acceptedOffer = _serviceRequestRepository.GetAcceptedOffer(report.Request.Id);
			report.ReporterName = acceptedOffer?.Provider?.FName + " " + acceptedOffer?.Provider?.LName;
			report.ReporterRole = "Provider";
			_context.Add(report);
			return Save();
		}

		public bool CreateProviderReport(Report report)
		{
			report.ReporterName = report.Request.Customer.FName + " " + report.Request.Customer.LName;
			report.ReporterRole = "Customer";
			_context.Add(report);
			return Save();
		}

		public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
