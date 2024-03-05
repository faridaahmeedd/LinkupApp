using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Dto.Reviews_Reports;
using AutoMapper;
using ServicesApp.APIs;

namespace ServicesApp.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository _ReportRepository;
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProviderRepository _providerRepository;

        public ReportController(IReportRepository ReportRepository, IMapper mapper, IProviderRepository providerRepository, ICustomerRepository customerRepository)
        {
            _ReportRepository = ReportRepository;
            _customerRepository = customerRepository;
            _providerRepository = providerRepository;
            _mapper = mapper;

        }

        [HttpGet]
        public IActionResult GetReports()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                var reports = _ReportRepository.GetReports();
                var mapreports = _mapper.Map<List<GetReportDto>>(reports);
                return Ok(mapreports);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpGet("{ReportId}")]
        public IActionResult GetReport(int ReportId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_ReportRepository.ReportExist(ReportId))
                {
                    return NotFound(ApiResponse.ReportNotFound);
                }
                var Report = _ReportRepository.GetReport(ReportId);
                var mapReport = _mapper.Map<GetReportDto>(Report);
                return Ok(mapReport);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpGet("CustomerReports/{CustomerId}")]
        public IActionResult GetCustomerReports(string CustomerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponse.ReportNotFound);
                }
                var Report = _ReportRepository.GetReportsOfCustomer(CustomerId);
                var mappedReports = Report.Select(report =>
                {
                    var reportDto = _mapper.Map<GetReportDto>(report);
                    reportDto.ReporterName = report.Customer.FName;

                    return reportDto;
                }).ToList();
                return Ok(mappedReports);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpGet("ProviderReports/{ProviderId}")]
        public IActionResult GetProviderReports(string ProviderId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponse.ReportNotFound);
                }
                var Report = _ReportRepository.GetReportsOfProvider(ProviderId);
                var mappedReports = Report.Select(report =>
                {
                    var reportDto = _mapper.Map<GetReportDto>(report);
                    reportDto.ReporterName = report.Provider.FName;

                    return reportDto;
                }).ToList();
                return Ok(mappedReports);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpPost("ReportCustomer/{CustomerId}/{ProviderId}")]
        public IActionResult CreateCustomerReport([FromBody] PostReportDto ReportCreate, string CustomerId, string ProviderId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReport = _mapper.Map<Report>(ReportCreate);
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReport.Customer = _customerRepository.GetCustomer(CustomerId);
                mapReport.Provider = _providerRepository.GetProvider(ProviderId);

                mapReport.ReporterName = mapReport.Provider.FName;
                mapReport.ReporterRole = "Provider";

                _ReportRepository.CreateReport(mapReport);
                return Ok(new
                {
                    statusMsg = "success",
                    message = "Report Created Successfully.",
                    ReportId = mapReport.Id,
                });
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpPost("ReportProvider/{CustomerId}/{ProviderId}")]
		public IActionResult CreateProviderReport([FromBody] PostReportDto ReportCreate, string CustomerId, string ProviderId)

        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReport = _mapper.Map<Report>(ReportCreate);
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReport.Customer = _customerRepository.GetCustomer(CustomerId);
                mapReport.Provider = _providerRepository.GetProvider(ProviderId);

                mapReport.ReporterName = mapReport.Customer.FName;

                mapReport.ReporterRole = "Customer";

                _ReportRepository.CreateReport(mapReport);
                return Ok(new
                {
                    statusMsg = "success",
                    message = "Report Created Successfully.",
                    ReportId = mapReport.Id,
                });
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }
    }
}