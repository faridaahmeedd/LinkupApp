using Microsoft.EntityFrameworkCore;
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

        public async Task<string> MatchJobAndService(int serviceId)
        {

            var request = _context.Requests.Find(serviceId);
            Console.WriteLine("-------------");
            Console.WriteLine(request.Id);


            var providers = _context.Providers.Select(provider => provider.Description).ToList();
            Console.WriteLine("-------------");
            Console.WriteLine(providers);
            var payload = new
            {
                service_description = request.Description , 
                job_descriptions = providers

              
            };
            foreach (var provider in providers) 
            {
                Console.WriteLine("-------------");

                Console.WriteLine(provider);
            }

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("http://127.0.0.1:5001/match", content);
            Console.WriteLine(response.Content);
            Console.WriteLine("----------------------");

            Console.WriteLine(response.IsSuccessStatusCode);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine("----------------------");
                Console.WriteLine(result);

                return result;
            }
            else
            {
                // Handle error
                return $"Error: {response.StatusCode}";
            }
        }



    }
}
