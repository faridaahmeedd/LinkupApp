using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ServicesApp.Data;
using ServicesApp.Interfaces;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ServicesApp.Repositories
{
    public class ML: IML
    {
        private readonly HttpClient _httpClient;
        private readonly DataContext _context;


        public ML( DataContext context)
        {
            _httpClient = new HttpClient();
            _context = context;


        }

        public async Task<bool> MatchJobAndService(int serviceId , string jobTitle)
        {

            var request = _context.Requests.Find(serviceId);
       
            Console.WriteLine("-------------");
            Console.WriteLine(request.Id);


           // var JobTitles = _context.Providers.Select(provider => provider.JobTitle).ToList();
            Console.WriteLine("-------------");
            var payload = new
            {
                jobtitles = jobTitle


            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://127.0.0.1:5000/predict", content);
            Console.WriteLine(response.Content);
            Console.WriteLine("----------------------");

            //Console.WriteLine(response.IsSuccessStatusCode);
            if (response.IsSuccessStatusCode)
            {

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var jsonObject = JObject.Parse(jsonResponse);
                var predictions = jsonObject["predictions"];
                foreach (var prediction in predictions)
                {
                    string predictionValue = prediction.ToString();
                    Console.WriteLine(predictionValue);

                    // Compare predictionValue with your string value
                    if (predictionValue == (request.Subcategory).ToString())
                    {

                        return true;
                    }
                }
                Console.WriteLine("----------------------");


                return false;
            }
            else
            {
                // Handle error
                return false;
            }
        }



    }
}
