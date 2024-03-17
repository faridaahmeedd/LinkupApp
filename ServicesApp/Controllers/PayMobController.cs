using Microsoft.AspNetCore.Mvc;
using ServicesApp.APIs;
using ServicesApp.Interfaces;
using ServicesApp.Repositories;

namespace ServicesApp.Controllers
{
    [ApiController]
    [Route("api/paymob")]
    public class PayMobController : ControllerBase
    {
        private readonly IPayMobRepository _payMobRepository;
        private readonly IServiceRequestRepository _serviceRepository;

        public PayMobController(IPayMobRepository payMobRepository , IServiceRequestRepository serviceRequestRepository )
        {
            _payMobRepository = payMobRepository;
            _serviceRepository = serviceRequestRepository;
            
        }
        [HttpPost]
        [Route("payment/{ServiceId}")]
        public async Task<IActionResult> PayService(int ServiceId)
        {
            try
            {
                if (!_serviceRepository.ServiceExist(ServiceId))
                {
                    return NotFound(ApiResponse.RequestNotFound);
                }
                if (_serviceRepository.GetAcceptedOffer(ServiceId) == null)
                {
                    return NotFound(ApiResponse.OfferNotFound);
                }

                var paymentLink=  await _payMobRepository.FirstStep(ServiceId);
                if (paymentLink != null)
                {
                    return Ok(paymentLink);
                }
                return BadRequest(ApiResponse.PaymentError);
            }
            catch
            {
                return StatusCode(500, ApiResponse.SomethingWrong);
            }
        }
    }
}
