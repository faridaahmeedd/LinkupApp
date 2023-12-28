﻿using Microsoft.AspNetCore.Mvc;
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
				return NotFound(ApiResponse.NotFoundUser);
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

                return NotFound(ApiResponse.NotFoundUser);
            }
            if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}

			if (!await _adminRepository.DeleteAdmin(AdminId))
			{
				return StatusCode(500, ApiResponse.SomthingWronge);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}
	}
}
