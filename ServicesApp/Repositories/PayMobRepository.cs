using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
using ServicesApp.Interfaces;
using System;
using ServicesApp.Models;
using Azure.Core;
using ServicesApp.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Web.Helpers;
using Azure;

namespace ServicesApp.Repositories
{
	public class PayMobRepository : IPayMobRepository
	{
		private readonly IConfiguration _configuration;
		private readonly IServiceRequestRepository _serviceRepository;
		private readonly DataContext _context;

		public PayMobRepository(IConfiguration configuration, IServiceRequestRepository serviceRequest, DataContext context)
		{
			_configuration = configuration;
			_serviceRepository = serviceRequest;
			_context = context;
		}

		const int integrationID = 4544296;   //online card old 4536584

		public async Task<string> auth()
		{
			var data = new { api_key = _configuration["PayMob:ApiKey"] };
			var response = await PostDataAndGetResponse("https://accept.paymob.com/api/auth/tokens", data, string.Empty);
			string token = response.token;
			return token.ToString();
		}

		public async Task<string> CardPayment(int ServiceId)
		{
			var request = _serviceRepository.GetService(ServiceId);
			var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
            var fees = offer.Examination ? (200 + request.Customer.Balance) : (offer.Fees + request.Customer.Balance);
            var data = new
			{
				amount = (100 *fees).ToString(),
				currency = "EGP",
				payment_methods = new[] { integrationID },
				billing_data = new
				{
					email = request.Customer.Email,
					first_name = request.Customer.FName,
					street = request.Customer.Address,
					phone_number = request.Customer.MobileNumber,
					city = request.Customer.City,
					country = request.Customer.Country,
					last_name = request.Customer.LName,
					state = "NA",
					apartment = "NA",
					floor = "NA",
					building = "NA",
					shipping_method = "NA",
					postal_code = "NA"
				},
				customer = new
				{
					first_name = request.Customer.FName,
					last_name = request.Customer.LName,
					email = request.Customer.Email
				}
			};

			var response = await PostDataAndGetResponse("https://accept.paymob.com/v1/intention/", data, _configuration["PayMob:SecretKey"]);
			string client_secret = response.client_secret;
			var iframeURL = $"https://accept.paymob.com/unifiedcheckout/?publicKey={_configuration["PayMob:PublicKey"]}&clientSecret={client_secret}";
			return iframeURL;
		}

		public async Task<bool> Capture(int TransactionId, int ServiceId)
		{
			var request = _serviceRepository.GetService(ServiceId);
			var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
            var fees = offer.Examination ? (200 + request.Customer.Balance) : (offer.Fees + request.Customer.Balance);

            string token = await auth();

			var captureData = new
			{
				transaction_id = TransactionId,
				amount_cents = (100 * fees).ToString(),
			};

			var Captureresponse = await PostDataAndGetResponse("https://accept.paymob.com/api/acceptance/capture", captureData, token);

			if (Captureresponse.success == "True")
			{
				request = _serviceRepository.GetService(ServiceId);
				request.PaymentStatus = "Paid";
				_serviceRepository.UpdateService(request);
				request.Customer.Balance = 0;
				_serviceRepository.Save();
			}
			return Captureresponse.success;
		}

		public async Task<dynamic> PostDataAndGetResponse(string url, object data, string token)
		{
			using (var client = new HttpClient())
			{
				var request = new HttpRequestMessage(HttpMethod.Post, url);
				if (!string.IsNullOrEmpty(token))
				{
					request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
				}
				var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
				request.Content = content;
				HttpResponseMessage response = null;
				try
				{
					response = await client.SendAsync(request);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error sending request: {ex.Message}");
					throw;
				}

				if (!response.IsSuccessStatusCode)
				{
					var errorContent = await response.Content.ReadAsStringAsync();
					throw new HttpRequestException($"Request failed with status code: {response.StatusCode}, content: {errorContent}");
				}
				var responseContent = await response.Content.ReadAsStringAsync();

				// Deserialize the response content to dynamic or any other type you expect
				dynamic result = JsonConvert.DeserializeObject(responseContent);
				return result;
			}
		}
	}
}
