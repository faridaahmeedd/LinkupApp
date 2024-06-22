using Microsoft.AspNetCore.Mvc;
using ServicesApp.Helper;
using ServicesApp.Interfaces;

[ApiController]
[Route("api/paypal")]
public class PayPalController : ControllerBase
{
	private readonly IPayPalRepository _payPalRepsoitory;
	private readonly IServiceRequestRepository _serviceRepository;

	public PayPalController(IPayPalRepository payPalRepsoitory, IServiceRequestRepository serviceRepository)
	{
		_payPalRepsoitory = payPalRepsoitory;
		_serviceRepository = serviceRepository;
	}

	[HttpPost("create-payment/{ServiceId}")]
	public async Task<IActionResult> CreatePayment(int ServiceId)
	{
		try
		{
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound(ApiResponses.RequestNotFound);
			}
			if(_serviceRepository.GetAcceptedOffer(ServiceId) == null)
			{
				return NotFound(ApiResponses.OfferNotFound);
			}
			var request = _serviceRepository.GetService(ServiceId);
			if(request.PaymentStatus == "Paid")
			{
				return BadRequest(ApiResponses.PaidAlready);
			}
			var approvalLink = await _payPalRepsoitory.CreatePayment(ServiceId);
			if(approvalLink != null)
			{
				return Ok(approvalLink);
			}
			return BadRequest(ApiResponses.PaymentError);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}

	[HttpPost("execute-payment/{ServiceId}/{PaymentId}/{Token}/{PayerId}")]
	public async Task<IActionResult> ExecutePayment(int ServiceId, string PaymentId, string Token, string PayerId)
	{
		try
		{
			if (!_serviceRepository.ServiceExist(ServiceId))
			{
				return NotFound(ApiResponses.RequestNotFound);
			}
			var responseState = await _payPalRepsoitory.ExecutePayment(ServiceId, PaymentId, Token, PayerId);
			if (responseState == "approved")
			{
				return Ok("http://localhost:7111/api/paypal/success-url");    // Redirect to your success page
			}
			return BadRequest(ApiResponses.PaymentError);
		}
		catch
		{
			return StatusCode(500, ApiResponses.SomethingWrong);
		}
	}
}