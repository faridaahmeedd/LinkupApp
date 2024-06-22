using Microsoft.AspNetCore.Mvc;
using ServicesApp.Helper;
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
                    return NotFound(ApiResponses.RequestNotFound);
                }
                if (_serviceRepository.GetAcceptedOffer(ServiceId) == null)
                {
                    return NotFound(ApiResponses.OfferNotFound);
                }
				var request = _serviceRepository.GetService(ServiceId);
				if (request.PaymentStatus == "Paid")
				{
					return BadRequest(ApiResponses.PaidAlready);
				}
				var paymentLink =  await _payMobRepository.CardPayment(ServiceId);
                if (paymentLink != null)
                {
                    return Ok(paymentLink);
                }
                return BadRequest(ApiResponses.PaymentError);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

        [HttpPost]
        [Route("Capture/{TransactionId}/{ServiceId}")]
        public async Task<IActionResult> CaptureTransaction(int TransactionId, int ServiceId)
        {
            try
            {
				if (!_serviceRepository.ServiceExist(ServiceId))
				{
					return NotFound(ApiResponses.RequestNotFound);
				}
				if (await _payMobRepository.Capture(TransactionId, ServiceId))
                {
                    return Ok(ApiResponses.CaptureSuccess);
                }
                return BadRequest(ApiResponses.CannotCapture);
            }
            catch
            {
                return StatusCode(500, ApiResponses.SomethingWrong);
            }
        }

		//[HttpPost]
		//[Route("Refund/{TransactionId}/{ServiceId}")]
		//public async Task<IActionResult> RefundService(int TransactionId, int ServiceId)
		//{
		//	try
		//	{

		//		if (await _payMobRepository.Refund(TransactionId, ServiceId))
		//		{
		//			return Ok(ApiResponses.RefundSuccess);
		//		}
		//		return BadRequest(ApiResponses.RefundedAlready);
		//	}
		//	catch
		//	{
		//		return StatusCode(500, ApiResponses.SomethingWrong);
		//	}
		//}
	}
}
