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

        public async Task<string> FirstStep(int ServiceId)
        {
            var data = new { api_key = _configuration["PayMob:ApiKey"] };
            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/auth/tokens", data);
            string token = response.token;
            Console.WriteLine(token.ToString());

            return await SecondStep(token.ToString(), ServiceId);
        }

        public async Task<string> SecondStep(string token , int ServiceId)
        {
            var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
            // var request = _serviceRepository.GetService(ServiceId);

            var data = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = (100 * offer.Fees).ToString(),
                currency = "EGP",
                items = new object[] { }
            };

            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/ecommerce/orders", data);
            int  id = response.id;
            Console.WriteLine(id);

            return  await ThirdStep(token.ToString(), id , ServiceId);
        }

        public async Task<string> ThirdStep(string token, int orderId , int ServiceId)
        {
            var request = _serviceRepository.GetService(ServiceId);
            var offer = _serviceRepository.GetAcceptedOffer(ServiceId);

            var data = new
            {
                auth_token = token,
                amount_cents = (100* offer.Fees).ToString(), 
                expiration = 3600,
                order_id = orderId,
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
                currency = "EGP",
                integration_id = integrationID
            };
            Console.WriteLine("---------------------");

           // Console.WriteLine(request.Customer.FName);
            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/acceptance/payment_keys", data);
            string theToken = response.token;
            Console.WriteLine(theToken.ToString());

            return await CardPayment(theToken.ToString());
        }

        public async Task<string> CardPayment(string token)
        {
            var iframeURL = $"https://accept.paymob.com/api/acceptance/iframes/831255?payment_token={token}";
            Console.WriteLine($"Redirecting to: {iframeURL}");
			
			return iframeURL;
        }

        public async Task<dynamic> PostDataAndGetResponse(string url, object data)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response content to dynamic or any other type you expect
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);

                return result;
            }
        }

        public async Task<bool> Capture(int TransactionId, int ServiceId)
        {
            var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
            var data = new { api_key = _configuration["PayMob:ApiKey"] };
            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/auth/tokens", data);
            string token = response.token;

            var captureData = new
            {
                auth_token = token,
                transaction_id = TransactionId,  
                amount_cents = (100 * offer.Fees).ToString(),
            };
            var Captureresponse = await PostDataAndGetResponse($"https://accept.paymob.com/api/acceptance/capture", captureData);

            if (Captureresponse.success == "True")
            {
				var request = _serviceRepository.GetService(ServiceId);
                request.PaymentStatus = "Paid";
                _serviceRepository.UpdateService(request);
            }
            return Captureresponse.success;
        }

        public async Task<bool> Refund(int TransactionId , int ServiceId)
		{
            var offer = _serviceRepository.GetAcceptedOffer(ServiceId);
            var request = _context.Requests.Include(c => c.Customer).Where(p => p.Id == ServiceId).FirstOrDefault();

            var data = new { api_key = _configuration["PayMob:ApiKey"] };
			var response = await PostDataAndGetResponse("https://accept.paymob.com/api/auth/tokens", data);
			string token = response.token;

            Console.WriteLine(token.ToString());

            var refundData = new {
				transaction_id = TransactionId,
                amount_cents = (100 * offer.Fees).ToString(),
            };
            
            var Refundresponse = await PostDataAndGetResponse($"https://accept.paymobsolutions.com/api/acceptance/void_refund/refund?token={token}", refundData);
            return Refundresponse.success;
		}
	}
}
