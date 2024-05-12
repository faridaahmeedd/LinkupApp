using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using System.Web.Helpers;
using ServicesApp.Helper;

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
					return BadRequest(ApiResponses.NotValid);
				}
				var Admins = await _adminRepository.GetAdmins();
				return Ok(Admins);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpGet("{AdminId}")]
		public async Task<IActionResult> GetAdmin(string AdminId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!await _adminRepository.AdminExist(AdminId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				var Admin = await _adminRepository.GetAdmin(AdminId);
				return Ok(Admin);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpDelete("{AdminId}")]
		public async Task<IActionResult> DeleteAdmin(string AdminId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!await _adminRepository.AdminExist(AdminId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				await _adminRepository.DeleteAdmin(AdminId);
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}
	}
}
