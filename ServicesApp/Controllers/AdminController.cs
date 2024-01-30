using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using System.Web.Helpers;
using ServicesApp.APIs;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class AdminController : ControllerBase
	{
		private readonly IAdminRepository _adminRepository;

		public AdminController(IAdminRepository adminRepository )
		{
			_adminRepository = adminRepository;
		}

		[HttpGet]
		public async Task<IActionResult> GetAdmins()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var Admins = await _adminRepository.GetAdmins();
			return Ok(Admins);
		}

		[HttpGet("{AdminId}")]
		public async Task<IActionResult> GetAdmin(string AdminId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (! await _adminRepository.AdminExist(AdminId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var Admin = await _adminRepository.GetAdmin(AdminId);
			return Ok(Admin);
		}

		[HttpDelete("{AdminId}")]
		public async Task<IActionResult> DeleteAdmin(string AdminId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!await _adminRepository.AdminExist(AdminId))
			{
                return NotFound(ApiResponse.UserNotFound);
            }
			if (!await _adminRepository.DeleteAdmin(AdminId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}
	}
}
