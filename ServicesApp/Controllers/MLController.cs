using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;
using ServicesApp.Repositories;

namespace ServicesApp.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class MLController : ControllerBase
    {
        private readonly IML _ml;

        public MLController(IML ml)
        {
            _ml = ml;
        }

        [HttpPost]
        public async Task<IActionResult> MatchJobAndService(int serviceId)
        {
            string result = await _ml.MatchJobAndService(serviceId);
            Console.WriteLine(result);

            return Ok(result);
        }
    }
}
