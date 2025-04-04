﻿using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using System.Net.Http.Headers;
using System.Text;

namespace ServicesApp.Repositories
{
	public class PayPalRepository : IPayPalRepository
	{
		private const string PayPalApiBaseUrl = "https://api.sandbox.paypal.com"; // Change to live URL for production

		private readonly HttpClient _httpClient;
		private readonly IConfiguration _configuration;
		private readonly DataContext _context;
		private readonly IServiceRequestRepository _serviceRepository;

		public PayPalRepository(IConfiguration configuration, DataContext context, IServiceRequestRepository serviceRepository)
		{
			_configuration = configuration;
			_context = context;
			_serviceRepository = serviceRepository;
			_httpClient = new HttpClient();
		}

		public async Task<string> CreatePayment(int ServiceId)
		{
			var request = _serviceRepository.GetService(ServiceId);
			var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
			var accessToken = await GetAccessToken();
			var createPaymentJson = new
			{
				intent = "sale",
				payer = new
				{
					payment_method = "paypal"
				},
				redirect_urls = new
				{
					return_url = "http://return.url",
					cancel_url = "http://cancel.url"
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
								name = "Linkup Service Fees",
								sku = request.Subcategory.NameEn,
								price = (offer.Fees + request.Customer.Balance).ToString(),
								currency = "USD",
								quantity = 1
							}
						}
					},
					amount = new
					{
						total = (offer.Fees + request.Customer.Balance).ToString(),
						currency = "USD"
					},
					description = request.Customer.Balance != 0 ? $"Service Fees: {offer.Fees} , Balance: {request.Customer.Balance}" : $"Service Fees: {offer.Fees}",
				}
			}
			};

			_httpClient.DefaultRequestHeaders.Remove("Authorization");   // Clear any existing authorization header
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

			var createPaymentResponse = await SendPayPalRequest("/v1/payments/payment", createPaymentJson);

			var approvalLink = GetApprovalLink(createPaymentResponse.links);
			return approvalLink;
		}

		public async Task<string> GetAccessToken()
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
			var responseContent = await response.Content.ReadAsStringAsync();

			if (response.IsSuccessStatusCode)
			{
				var tokenData = JsonConvert.DeserializeObject<dynamic>(responseContent);
				return tokenData.access_token;
			}
			throw new Exception($"Failed to retrieve PayPal access token. Response: {responseContent}");
		}

		public string GetApprovalLink(dynamic links)
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

		public async Task<dynamic> SendPayPalRequest(string endpoint, object requestData)
		{
			var requestJson = JsonConvert.SerializeObject(requestData);

			var fullUrl = new Uri(new Uri(PayPalApiBaseUrl), endpoint);

			// Set authorization header
			var accessToken = await GetAccessToken();
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

			var response = await _httpClient.PostAsync(fullUrl, new StringContent(requestJson, Encoding.UTF8, "application/json"));

			if (response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<dynamic>(responseBody);
			}

			// Handle error response
			var errorResponse = await response.Content.ReadAsStringAsync();
			throw new Exception($"PayPal API request failed. Status code: {response.StatusCode}. Response: {errorResponse}");
		}

		public async Task<string> ExecutePayment(int ServiceId, string paymentId, string token, string payerID)
		{
			var executePaymentJson = new
			{
				payer_id = payerID,
			};
			var request = _serviceRepository.GetService(ServiceId);
			var executePaymentResponse = await SendPayPalRequest($"/v1/payments/payment/{paymentId}/execute?token={token}", executePaymentJson);
			if (executePaymentResponse.state == "approved")
			{
				request.PaymentStatus = "Paid";
				_serviceRepository.UpdateService(request);
				request.Customer.Balance = 0;
				_serviceRepository.Save();
			}
			return executePaymentResponse.state;
		}
	}
}
