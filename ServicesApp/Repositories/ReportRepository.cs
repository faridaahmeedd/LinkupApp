using Microsoft.EntityFrameworkCore;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using System.Linq;

namespace ServicesApp.Repositories
{
	public class ReportRepository : IReportRepository
    {
        private readonly DataContext _context;

        public ReportRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Report> GetReports()
        {
            return _context.Reports.OrderBy(p => p.Id).ToList();
        }

        public ICollection<Report> GetReportsOfCustomer(string customerId)
        {
            return _context.Reports
               .Include(r => r.request)
               .Where(r => r.request.Customer.Id == customerId && r.ReporterRole == "Provider")
               .OrderBy(r => r.Id)
               .ToList();
        }

        public ICollection<Report> GetReportsOfProvider(string providerId)
        {
            var offers = _context.Offers.Include(o => o.Request).Where(o => o.Provider.Id == providerId).ToList();

            var requestIds = offers.Select(o => o.Request.Id).ToList();

            var reports = _context.Reports
                .Include(r => r.request)
                    .ThenInclude(req => req.Customer)
                .Where(r => requestIds.Contains(r.request.Id) && r.ReporterRole == "Customer")
                .OrderBy(r => r.Id)
                .ToList();

            return reports;
        }

        public Report GetReport(int id)
        {
            return _context.Reports.Where(p => p.Id == id).FirstOrDefault();
        }

        public bool ReportExist(int id)
        {
            return _context.Reports.Any(p => p.Id == id);
        }

        public bool CreateReport(Report report)
        {
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
