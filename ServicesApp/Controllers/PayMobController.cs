using Microsoft.AspNetCore.Mvc;
using ServicesApp.Repositories;

namespace ServicesApp.Controllers
{
    [ApiController]
    [Route("api/paypal")]
    public class PayMobController : ControllerBase
    {
        private readonly PayMobRepository _payMobRepository;
        public PayMobController(PayMobRepository payMobRepository)
        {
            _payMobRepository = payMobRepository;
            
        }
        [HttpPost]
        [Route("firstStep")]
        public async Task<IActionResult> FirstStep()
        {
            try
            {
                 await _payMobRepository.FirstStep();
                return Ok("SUCCESS"); // You can customize the response as needed
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
