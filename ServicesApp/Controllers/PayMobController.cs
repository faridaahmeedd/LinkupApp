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
        [Route("CardPayment/{ServiceId}")]
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
				var request = _serviceRepository.GetService(ServiceId);
				if (request.PaymentStatus == "Paid")
				{
					return BadRequest(ApiResponse.PaidAlready);
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

		[HttpPost]
		[Route("Refund/{TransactionId}/{ServiceId}")] 
		public async Task<IActionResult> RefundService(int TransactionId , int ServiceId)
		{
			try
			{
				if (await _payMobRepository.Refund(TransactionId , ServiceId))
				{
					return Ok(ApiResponse.RefundSuccess);
				}
				return BadRequest(ApiResponse.RefundedAlready);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
	}
}
