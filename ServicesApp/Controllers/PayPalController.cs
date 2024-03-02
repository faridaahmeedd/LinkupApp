using Microsoft.AspNetCore.Mvc;
using ServicesApp.APIs;
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
				return NotFound(ApiResponse.RequestNotFound);
			}
			if(_serviceRepository.GetAcceptedOffer(ServiceId) == null)
			{
				return NotFound(ApiResponse.OfferNotFound);
			}
			var approvalLink = await _payPalRepsoitory.CreatePayment(ServiceId);
			if(approvalLink != null)
			{
				return Ok(approvalLink);
			}
			return BadRequest(ApiResponse.PaymentError);
		}
		catch
		{
			return StatusCode(500, ApiResponse.SomethingWrong);
		}
	}

	[HttpPost("execute-payment/{PaymentId}/{Token}/{PayerId}")]
	public async Task<IActionResult> ExecutePayment(string PaymentId, string Token, string PayerId)
	{
		try
		{
			var responseState = await _payPalRepsoitory.ExecutePayment(PaymentId, Token, PayerId);
			if (responseState == "approved")
			{
				// Redirect to your success page
				return Ok("http://localhost:7111/api/paypal/success-url");
			}
			else
			{
				return BadRequest(ApiResponse.PaymentError);
			}
		}
		catch
		{
			return StatusCode(500, ApiResponse.SomethingWrong);
		}
	}
}