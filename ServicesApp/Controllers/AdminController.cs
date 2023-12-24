using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IAdminRepository _adminRepository;

		public AdminController(IAdminRepository adminRepository)
		{
			_adminRepository = adminRepository;
		}

		[HttpGet]
		[ProducesResponseType(200, Type = typeof(Admin))]
		public IActionResult GetAdmins()
		{
			var Admins = _adminRepository.GetAdmins();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Admins);
		}

		//when use async
        /*[HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Admin>))]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _adminRepository.GetAdminsAsync();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(admins);
        }
		*/


        [HttpGet("{AdminId}")]
		[ProducesResponseType(200, Type = typeof(Admin))]
		public IActionResult GetAdmin(string AdminId)
		{
			if (!_adminRepository.AdminExist(AdminId))
			{
				return NotFound();
			}
			var Admin = _adminRepository.GetAdmin(AdminId);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Admin);
		}

		//[HttpGet("Login")]
		//[ProducesResponseType(200, Type = typeof(Admin))]
		//public IActionResult Login([FromQuery] string email, [FromQuery] string password)
		//{
		//	var Admin = _adminRepository.GetAdmin(email, password);
		//	if (Admin == null)
		//	{
		//		return NotFound();
		//	}
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}
		//	return Ok(Admin);
		//}
	}
}
