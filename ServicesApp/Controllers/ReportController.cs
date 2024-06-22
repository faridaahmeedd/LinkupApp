using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Dto.Reviews_Reports;
using AutoMapper;
using ServicesApp.Repository;
using ServicesApp.Helper;

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
        private readonly IServiceRequestRepository _serviceRequestRepository;

        public ReportController(IReportRepository ReportRepository, IMapper mapper, 
            IProviderRepository providerRepository, ICustomerRepository customerRepository, 
            IServiceRequestRepository serviceRequestRepository)
        {
            _ReportRepository = ReportRepository;
            _customerRepository = customerRepository;
            _providerRepository = providerRepository;
			_serviceRequestRepository = serviceRequestRepository;
			_mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetReports()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                var reports = _ReportRepository.GetReports();
                var mapreports = _mapper.Map<List<GetReportDto>>(reports);
                return Ok(mapreports);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("{ReportId}")]
        public IActionResult GetReport(int ReportId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_ReportRepository.ReportExist(ReportId))
                {
                    return NotFound(ApiResponses.ReportNotFound);
                }
                var Report = _ReportRepository.GetReport(ReportId);
                var mapReport = _mapper.Map<GetReportDto>(Report);
                return Ok(mapReport);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("CustomerReports/{CustomerId}")]
        public IActionResult GetCustomerReports(string CustomerId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_customerRepository.CustomerExist(CustomerId))
                {
                    return NotFound(ApiResponses.ReportNotFound);
                }
                var Report = _ReportRepository.GetReportsOfCustomer(CustomerId);
                var mappedReports = Report.Select(report =>
                {
                    var reportDto = _mapper.Map<GetReportDto>(report);
                    return reportDto;
                }).ToList();

                return Ok(mappedReports);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpGet("ProviderReports/{ProviderId}")]
        public IActionResult GetProviderReports(string ProviderId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_providerRepository.ProviderExist(ProviderId))
                {
                    return NotFound(ApiResponses.ReportNotFound);
                }
                var Report = _ReportRepository.GetReportsOfProvider(ProviderId);
                var mappedReports = Report.Select(report =>
                {
                    var reportDto = _mapper.Map<GetReportDto>(report);
                    return reportDto;
                }).ToList();

                return Ok(mappedReports);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpPost("Customer/{RequestId}")]
        public IActionResult CreateCustomerReport([FromBody] PostReportDto ReportCreate, int RequestId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_serviceRequestRepository.ServiceExist(RequestId))
                {
                    return NotFound(ApiResponses.RequestNotFound);
                }
				//if (!_serviceRequestRepository.CheckRequestCompleted(RequestId))
				//{
				//	return BadRequest(ApiResponses.UncompletedService);
				//}

				var mapReport = _mapper.Map<Report>(ReportCreate);
				mapReport.Request = _serviceRequestRepository.GetService(RequestId);
                _ReportRepository.CreateCustomerReport(mapReport);

                return Ok(new
                {
                    statusMsg = "success",
                    message = "Report Created Successfully.",
                    ReportId = mapReport.Id,
                });
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpPost("Provider/{RequestId}")]
        public IActionResult CreateProviderReport([FromBody] PostReportDto ReportCreate, int RequestId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponses.NotValid);
                }
                if (!_serviceRequestRepository.ServiceExist(RequestId))
                {
                    return NotFound(ApiResponses.RequestNotFound);
                }
				//if (!_serviceRequestRepository.CheckRequestCompleted(RequestId))
				//{
				//	return BadRequest(ApiResponses.UncompletedService);
				//}

				var mapReport = _mapper.Map<Report>(ReportCreate);
				mapReport.Request = _serviceRequestRepository.GetService(RequestId);
                _ReportRepository.CreateProviderReport(mapReport);

                return Ok(new
                {
                    statusMsg = "success",
                    message = "Report Created Successfully.",
                    ReportId = mapReport.Id,
                });
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }
    }
}