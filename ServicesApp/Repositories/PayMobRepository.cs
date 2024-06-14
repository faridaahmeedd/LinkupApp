using Nager.Country;
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

        public PayMobRepository(IConfiguration configuration , IServiceRequestRepository serviceRequest, DataContext context)
        {
            _configuration = configuration;
            _serviceRepository = serviceRequest;
            _context = context;
        }
        
        const int integrationID = 4544296;   //online card old 4536584

        public async Task<string> auth()
        {
            var data = new { api_key = _configuration["PayMob:ApiKey"] };
            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/auth/tokens/", data, String.Empty);
            string token = response.token;
            return token.ToString();
        }

        public async Task<string> CardPayment(int ServiceId)
        {
            var request = _serviceRepository.GetService(ServiceId);
            var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
            var data = new
            {
                amount = (100 * (offer.Fees + request.Customer.Balance)).ToString(),
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
                    // TODO : not real data , can not reomve
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

            string token = await auth();


			Console.WriteLine("-------------------token-----------------------------");
			Console.WriteLine(token);

			var captureData = new
            {
                transaction_id = TransactionId,  
                amount_cents = (100 * (offer.Fees + request.Customer.Balance)).ToString(),
            };
            var Captureresponse = await PostDataAndGetResponse("https://accept.paymob.com/api/acceptance/capture/", captureData, token);

            if (Captureresponse.success == "True")
            {

				Console.WriteLine("-------------------capture----------------------------");
				//Console.WriteLine(client_secret);
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
				Console.WriteLine("-------------------token-----------------------------");
				Console.WriteLine(token);
				var request = new HttpRequestMessage(HttpMethod.Post, url);
				if (token != String.Empty)
                {
					request.Headers.Add("Authorization", $"Token {token}");
				}
				var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
				Console.WriteLine("-------------------conetnt-----------------------------");
				request.Content = content;
				var response = await client.SendAsync(request);
				Console.WriteLine("-------------------send-----------------------------");
				response.EnsureSuccessStatusCode();
				Console.WriteLine(response.EnsureSuccessStatusCode());
				var responseContent = await response.Content.ReadAsStringAsync();
				dynamic result = JsonConvert.DeserializeObject(responseContent);
				return result;
			}
		}



		//public async Task<bool> Refund(int TransactionId , int ServiceId)
		//{
		//    var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
		//
		//    var data = new { api_key = _configuration["PayMob:ApiKey"] };
		//	var response = await PostDataAndGetResponse("https://accept.paymob.com/api/auth/tokens", data);
		//	string token = response.token;
		//    var refundData = new {
		//		transaction_id = TransactionId,
		//        amount_cents = (100 * offer.Fees).ToString(),
		//    }; 
		//    var Refundresponse = await PostDataAndGetResponse($"https://accept.paymobsolutions.com/api/acceptance/void_refund/refund?token={token}", refundData);
		//    return Refundresponse.success;
		//}
	}
}
