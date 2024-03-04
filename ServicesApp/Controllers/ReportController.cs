using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repository;
using ServicesApp.Dto.Reviews_Reports;
using ServicesApp.Core.Models;
using AutoMapper;
using ServicesApp.APIs;
using Microsoft.AspNetCore.Identity;
using ServicesApp.Repositories;

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

        [HttpGet("{ReportId:int}", Name = "GetReportById")]
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
        [HttpGet("GetCustomerReports/{customerId}")]
        public IActionResult GetCustomerReports(string customerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_customerRepository.CustomerExist(customerId))
                {
                    return NotFound(ApiResponse.ReportNotFound);
                }
                var Report = _ReportRepository.GetReportsOfCustomer(customerId);
                var mappedReports = Report.Select(report =>
                {
                    var reportDto = _mapper.Map<GetReportDto>(report);

                    // Set the ReporterName based on the customer's name
                    reportDto.ReporterName = report.Customer.FName;

                    return reportDto;
                }).ToList();
                // var mapReview = _mapper.Map<List<GetReviewDto>>(Review);
                return Ok(mappedReports);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }
        [HttpGet("GetProviderReports/{providerId}")]
        public IActionResult GetProviderReports(string providerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }
                if (!_providerRepository.ProviderExist(providerId))
                {
                    return NotFound(ApiResponse.ReportNotFound);
                }
                var Report = _ReportRepository.GetReportsOfProvider(providerId);
                var mappedReports = Report.Select(report =>
                {
                    var reportDto = _mapper.Map<GetReportDto>(report);

                    // Set the ReviewerName based on the customer's name
                    reportDto.ReporterName = report.Provider.FName;

                    return reportDto;
                }).ToList();
                // var mapReview = _mapper.Map<List<GetReviewDto>>(Review);
                return Ok(mappedReports);

            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }

        [HttpPost("ReportCustomer")]
        public IActionResult CreateCustomerReport([FromBody] PostReportDto ReportCreate, string customerId, string providerId)

        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReport = _mapper.Map<Report>(ReportCreate);
                if (!_customerRepository.CustomerExist(customerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(providerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReport.Customer = _customerRepository.GetCustomer(customerId);
                mapReport.Provider = _providerRepository.GetProvider(providerId);

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



        [HttpPost("reportProvider")]
        public IActionResult CreateProviderReport([FromBody] PostReportDto ReportCreate, string customerId, string providerId)

        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse.NotValid);
                }

                var mapReport = _mapper.Map<Report>(ReportCreate);
                if (!_customerRepository.CustomerExist(customerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                if (!_providerRepository.ProviderExist(providerId))
                {
                    return NotFound(ApiResponse.UserNotFound);
                }
                mapReport.Customer = _customerRepository.GetCustomer(customerId);
                mapReport.Provider = _providerRepository.GetProvider(providerId);

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

