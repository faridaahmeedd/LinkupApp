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
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var Admins = await _adminRepository.GetAdmins();
				return Ok(Admins);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpGet("{AdminId}")]
		public async Task<IActionResult> GetAdmin(string AdminId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!await _adminRepository.AdminExist(AdminId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				var Admin = await _adminRepository.GetAdmin(AdminId);
				return Ok(Admin);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpDelete("{AdminId}")]
		public async Task<IActionResult> DeleteAdmin(string AdminId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!await _adminRepository.AdminExist(AdminId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				await _adminRepository.DeleteAdmin(AdminId);
				return Ok(ApiResponse.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
	}
}
