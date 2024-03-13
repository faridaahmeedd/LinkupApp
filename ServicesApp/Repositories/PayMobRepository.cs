using Newtonsoft.Json;
using System.Text;

namespace ServicesApp.Repositories
{
    public class PayMobRepository
    {
        private readonly IConfiguration _configuration;

        public PayMobRepository(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        public async Task<string> GenerateToken()
        {
            var api_key = _configuration["PayMob:ApiKey"];
          
            using (HttpClient client = new HttpClient())
            {
                var data = new
                {
                    api_key = api_key
                };

                var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://accept.paymob.com/api/auth/tokens", content);
                Console.WriteLine("-----------ssssssssss--------");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var token = JsonConvert.DeserializeObject<dynamic>(jsonString);
                    Console.WriteLine("-------------------");

                    Console.WriteLine(token); // or Console.WriteLine(token.ToString());

                    string link = await GeneratePaymentLinkAsync(token.token.ToString());
                    Console.WriteLine("-------------------");

                    Console.WriteLine(link);
                    return link;
                }
                else
                {
                    // Handle error if the request was not successful
                    Console.WriteLine($"Error: {response.StatusCode}");
                    return "error";
                }
            }
        }
        public async Task<string> GeneratePaymentLinkAsync(string token)
        {
            // Replace 'YOUR_INTEGRATION_ID' with your configured integration ID
            var integrationId = "4538914";

            // Replace with the necessary payment details (amount in cents, Name, email, and Phone number)
            var paymentDetails = new
            {
                payment_method = integrationId,
                
                amount_cents = 1000, // Replace with the actual amount in cents
                billing_name = "Linkup", // Replace with the actual name
                billing_email = "linkupp2024@gmail.com", // Replace with the actual email
                billing_phone = "01152034147",// Replace with the actual phone number
                                              // Add any other necessary fields
                is_live = true, // Add this field if required
                integrations = new[] {"PayPal"  },

                // Add the required payment_methods field (replace "credit_card" with the actual payment method)
                payment_methods = new[] { 831 , 10 , 832 , 6, 158},


            };

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var content = new StringContent(JsonConvert.SerializeObject(paymentDetails), Encoding.UTF8, "application/json");
                Console.WriteLine($"Request Payload: {await content.ReadAsStringAsync()}");

                using (var response = await httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/payment-links", content))
                {
                    Console.WriteLine(response);

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorResponse = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error Response: {errorResponse}");
                    }

                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var paymentLinkResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);

                    // Assuming the payment link is a string property in the response, replace 'PropertyName' with the actual property name.
                    var paymentLink = paymentLinkResponse.PropertyName?.ToString();

                    Console.WriteLine("------shrook--------------");

                    return paymentLink;
                }
            }
        }

        //public  async Task<string> GeneratePaymentLinkAsync(string token)
        //{
        //    // Replace 'YOUR_INTEGRATION_ID' with your configured integration ID
        //    string integrationId = "4536584";

        //    // Replace with the necessary payment details (amount in cents, Name, email, and Phone number)
        //    var paymentDetails = new
        //    {
        //        payment_method = integrationId,
        //        amount_cents = 1000, // Replace with the actual amount in cents
        //        billing_name = "John Doe", // Replace with the actual name
        //        billing_email = "john.doe@example.com", // Replace with the actual email
        //        billing_phone = "1234567890" // Replace with the actual phone number
        //                                     // Add any other necessary fields
        //    };

        //    using (var httpClient = new HttpClient())
        //    {
        //        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

        //        var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(paymentDetails), Encoding.UTF8, "application/json");

        //        using (
        //            var response = await httpClient.PostAsync("https://accept.paymob.com/api/ecommerce/payment-links", content))
        //        {
        //            if (!response.IsSuccessStatusCode)
        //            {
        //                var errorResponse = await response.Content.ReadAsStringAsync();
        //                Console.WriteLine($"Error Response: {errorResponse}");
        //            }
        //            Console.WriteLine("-----fefe--------------");
        //           // response.EnsureSuccessStatusCode();

        //            var jsonResponse = await response.Content.ReadAsStringAsync();
        //            var paymentLinkResponse = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
        //            var paymentLink = paymentLinkResponse;

        //            Console.WriteLine(paymentLink);
        //            Console.WriteLine("------shrook--------------");

        //            return paymentLink;
        //        }
        //    }
        //}


    }
}
