using Microsoft.AspNetCore.Mvc;
using ServicesApp.Interfaces;
using System.Web.Helpers;
using ServicesApp.APIs;
using Microsoft.AspNetCore.Authorization;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	[Authorize(Roles = "MainAdmin")]
	public class AdminController : ControllerBase
	{
		private readonly IAdminRepository _adminRepository;

		public AdminController(IAdminRepository adminRepository )
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
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(Admins);
		}

		[HttpGet("{AdminId}")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> GetAdmin(string AdminId)
		{
			if (! await _adminRepository.AdminExist(AdminId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var Admin = await _adminRepository.GetAdmin(AdminId);
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			return Ok(Admin);
		}

		[HttpDelete("{AdminId}")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> DeleteAdmin(string AdminId)
		{
			if (!await _adminRepository.AdminExist(AdminId))
			{

                return NotFound(ApiResponse.UserNotFound);
            }
            if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}

			if (!await _adminRepository.DeleteAdmin(AdminId))
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}
	}
}
