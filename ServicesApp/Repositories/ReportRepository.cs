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
            return _context.Reports.Include(p => p.Customer).Where(p => p.ReporterRole == "Provider" && p.Customer.Id == customerId).OrderBy(p => p.Id).ToList();
        }

        public ICollection<Report> GetReportsOfProvider(string providerId)
        {
            return _context.Reports.Include(p => p.Provider).Where(p => p.ReporterRole == "Customer" && p.Provider.Id == providerId).OrderBy(p => p.Id).ToList();
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
