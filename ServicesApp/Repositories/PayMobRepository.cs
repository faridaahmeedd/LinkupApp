using Nager.Country;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http;
namespace ServicesApp.Repositories
{
    public class PayMobRepository
    {
        private readonly IConfiguration _configuration;

        public PayMobRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //const string APIKey = "ZXlKaGJHY2lPaUpJVXpVeE1pSXNJblI1Y0NJNklrcFhWQ0o5LmV5SmpiR0Z6Y3lJNklrMWxjbU5vWVc1MElpd2ljSEp2Wm1sc1pWOXdheUk2T1RZMU1ESXlMQ0p1WVcxbElqb2lhVzVwZEdsaGJDSjkuTVJHc2lYMDFlNE04STQ4ekpNRk9CaHhOMzBTUDlEUV92N1RVdVlfMnBLUl9veDRvdXJ4TlJmWFVtVFJ0OTJleGJrN0RDVE9SZUhNTW40M3R5eWw0TkE="; 
        const int integrationID = 4536584;   //credit

        public async Task FirstStep()
        {
            var data = new { api_key = _configuration["PayMob:ApiKey"] };
            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/auth/tokens", data);
            string token = response.token;
            Console.WriteLine(token.ToString());

            await SecondStep(token.ToString());
        }

        public async Task SecondStep(string token)
        {
            var data = new
            {
                auth_token = token,
                delivery_needed = "false",
                amount_cents = "1000",
                currency = "EGP",
                items = new object[] { }
            };

            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/ecommerce/orders", data);
            int  id = response.id;
            Console.WriteLine(id);

            await ThirdStep(token.ToString(), id);
        }

        public async Task ThirdStep(string token, int orderId)
        {
            var data = new
            {
                auth_token = token,
                amount_cents = "1000",
                expiration = 3600,
                order_id = orderId,
                billing_data = new
                {
                    apartment = "803",
                    email = "claudette09@exa.com",
                    floor = "42",
                    first_name = "Clifford",
                    street = "Ethan Land",
                    building = "8028",
                    phone_number = "+86(8)9135210487",
                    shipping_method = "PKG",
                    postal_code = "01898",
                    city = "Jaskolskiburgh",
                    country = "CR",
                    last_name = "Nicolas",
                    state = "Utah"
                },
                currency = "EGP",
                integration_id = integrationID
            };

            var response = await PostDataAndGetResponse("https://accept.paymob.com/api/acceptance/payment_keys", data);
            string theToken = response.token;
            Console.WriteLine(theToken.ToString());

            await CardPayment(theToken.ToString());
        }

        public async Task CardPayment(string token)
        {
            var iframeURL = $"https://accept.paymob.com/api/acceptance/iframes/831255?payment_token={token}";
            Console.WriteLine($"Redirecting to: {iframeURL}");
        }


        public async Task<dynamic> PostDataAndGetResponse(string url, object data)
        {
            using (var client = new HttpClient())
            {
                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data), System.Text.Encoding.UTF8, "application/json");
                var response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();

                // Read the response content as a string
                var responseContent = await response.Content.ReadAsStringAsync();

                // Deserialize the response content to dynamic or any other type you expect
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(responseContent);

                return result;
            }
        }


    }
}
