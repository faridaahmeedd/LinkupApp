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
	public class MLRepository : IMLRepository
	{
		private readonly HttpClient _httpClient;
		private readonly DataContext _context;

		public MLRepository(DataContext context)
		{
			_httpClient = new HttpClient();
			_context = context;
		}

		public async Task<bool> MatchJobAndService(int serviceId, string providerId)
		{
			var request = _context.Requests.Include(p => p.Subcategory).Where(p => p.Id == serviceId).FirstOrDefault();
			var provider = _context.Providers.Where(p => p.Id == providerId).FirstOrDefault();

			if (request == null || provider == null)
			{
				return false;
			}

			var payload = new
			{
				jobtitle = provider.JobTitle
			};

			var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

			var response = await _httpClient.PostAsync("http://127.0.0.1:5000/predict", content);

			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = await response.Content.ReadAsStringAsync();
				var jsonObject = JObject.Parse(jsonResponse);
				var predictedSubcategory = jsonObject["subcategory"].ToString();
				Console.WriteLine(predictedSubcategory);

				if (predictedSubcategory == request.Subcategory.Name)
				{
					return true;
				}
			}

			return false;
		}
	}
}
