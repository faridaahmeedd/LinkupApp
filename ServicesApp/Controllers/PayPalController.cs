using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

[ApiController]
[Route("api/paypal")]
public class PayPalController : ControllerBase
{
	private const string PayPalApiBaseUrl = "https://api.sandbox.paypal.com"; // Change to live URL for production

	private readonly HttpClient _httpClient;
	private readonly IConfiguration _configuration;


	public PayPalController(IConfiguration configuration)
	{
		_httpClient = new HttpClient();
		_configuration = configuration;
	}

	private async Task<string> GetAccessToken()
	{
		var clientId = _configuration["PayPal:ClientId"];
		var clientSecret = _configuration["PayPal:ClientSecret"];

		var tokenRequest = new List<KeyValuePair<string, string>>
	{
		new KeyValuePair<string, string>("grant_type", "client_credentials")
	};

		var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));
		_httpClient.DefaultRequestHeaders.Clear();
		_httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
		_httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {base64Auth}");

		var response = await _httpClient.PostAsync("https://api.sandbox.paypal.com/v1/oauth2/token", new FormUrlEncodedContent(tokenRequest));
		Console.WriteLine(response.ToString());
		var responseContent = await response.Content.ReadAsStringAsync();

		if (response.IsSuccessStatusCode)
		{
			var tokenData = JsonConvert.DeserializeObject<dynamic>(responseContent);
			return tokenData.access_token;
		}

		throw new Exception($"Failed to retrieve PayPal access token. Response: {responseContent}");
	}

	[HttpPost("create-payment")]
	public async Task<IActionResult> CreatePayment()
	{
		var accessToken = await GetAccessToken();
		Console.WriteLine(accessToken);
		var createPaymentJson = new
		{
			intent = "authorize",
			payer = new
			{
				payment_method = "paypal"
			},
			redirect_urls = new
			{
				return_url = "http://return.url",
				cancel_url = "http://localhost:7111/api/paypal/execute/"
			},
			transactions = new[]
			{
				new
				{
					item_list = new
					{
						items = new[]
						{
							new
							{
								name = "item",
								sku = "item",
								price = "1.00",
								currency = "USD",
								quantity = 1
							}
						}
					},
					amount = new
					{
						currency = "USD",
						total = "1.00"
					},
					description = "This is the payment description."
				}
			}
		};



		_httpClient.DefaultRequestHeaders.Remove("Authorization"); // Clear any existing authorization header
		_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
		Console.WriteLine($"Request Headers: {string.Join(", ", _httpClient.DefaultRequestHeaders)}");


		Console.WriteLine($"Create Payment JSON: {JsonConvert.SerializeObject(createPaymentJson)}");

		var createPaymentResponse = await SendPayPalRequest("/v1/payments/payment", createPaymentJson);

		Console.WriteLine($"Create Payment Response: {JsonConvert.SerializeObject(createPaymentResponse)}");

		var approvalLink = GetApprovalLink(createPaymentResponse.links);
		return Ok(approvalLink);
		//return Ok(JsonConvert.SerializeObject(createPaymentResponse));
	}

	private string GetApprovalLink(dynamic links)
	{
		foreach (var link in links)
		{
			if (link.rel == "approval_url")
			{
				return link.href;
			}
		}
		throw new Exception("Approval link not found in the PayPal API response.");
	}

	[HttpPost("execute")]
	public async Task<IActionResult> ExecutePayment([FromQuery] string paymentId, [FromQuery] string token, [FromQuery] string PayerID)
	{
		try
		{
			var executePaymentJson = new
			{
				payer_id = PayerID,
				transactions = new[]
				{
					new
					{
						amount = new
						{
							currency = "USD",
							total = "1.00"
						}
					}
				}
			};

			var executePaymentResponse = await SendPayPalRequest($"/v1/payments/payment/{paymentId}/execute?token={token}", executePaymentJson);

			// Check if the payment execution is successful
			if (executePaymentResponse.state == "approved")
			{
				// Redirect to your success page
				return Ok("http://localhost:7111/api/paypal/success-url");
			}
			else
			{
				// Handle unsuccessful payment execution
				return BadRequest($"Payment execution failed. Payment state: {executePaymentResponse.state}");
			}
		}
		catch (Exception ex)
		{
			// Handle exceptions
			return BadRequest($"Error executing payment: {ex.Message}");
		}
	}

	private async Task<dynamic> SendPayPalRequest(string endpoint, object requestData)
	{
		var requestJson = JsonConvert.SerializeObject(requestData);
		var response = await _httpClient.PostAsync($"{PayPalApiBaseUrl}{endpoint}", new StringContent(requestJson, Encoding.UTF8, "application/json"));
		Console.WriteLine($"responsee: {response}");
		Console.WriteLine($"Request JSON: {requestJson}");


		if (response.IsSuccessStatusCode)
		{
			var responseBody = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<dynamic>(responseBody);
		}

		// Handle error response
		var errorResponse = await response.Content.ReadAsStringAsync();
		throw new Exception($"PayPal API request failed. Status code: {response.StatusCode}. Response: {errorResponse}");
	}
}
