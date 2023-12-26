using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;

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
		[ProducesResponseType(200)]
		public async Task<IActionResult> GetAdmins()
		{
			var Admins = await _adminRepository.GetAdmins();
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Admins);
		}

		[HttpGet("{AdminId}")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> GetAdmin(string AdminId)
		{
			if (! await _adminRepository.AdminExist(AdminId))
			{
				return NotFound();
			}
			var Admin = await _adminRepository.GetAdmin(AdminId);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(Admin);
		}

		[HttpDelete("{AdminId}")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> DeleteAdmin(string AdminId)
		{
			if (!await _adminRepository.AdminExist(AdminId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (!await _adminRepository.DeleteAdmin(AdminId))
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, ModelState);
			}
			return Ok("Successfully deleted");
		}
	}
}
