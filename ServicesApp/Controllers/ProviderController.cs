using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.APIs;
using ServicesApp.Dto.Users;
using ServicesApp.Interfaces;
using ServicesApp.Models;

namespace ServicesApp.Controllers
{
	[Route("/api/[controller]")]
	[ApiController]
	public class ProviderController : ControllerBase
	{
		private readonly IProviderRepository _providerRepository;
		private readonly IMapper _mapper;

		public ProviderController(IProviderRepository ProviderRepository, IMapper mapper)
		{
			_providerRepository = ProviderRepository;
			_mapper = mapper;
		}


		[HttpGet]
		public IActionResult GetProviders()
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			var providers = _providerRepository.GetProviders();
			var mapProviders = _mapper.Map<List<ProviderDto>>(providers);
			return Ok(mapProviders);
		}


		[HttpGet("{ProviderId}", Name = "GetProviderById")]
		public IActionResult GetProvider(string ProviderId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_providerRepository.ProviderExist(ProviderId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var provider = _providerRepository.GetProvider(ProviderId);
			var mapProvider = _mapper.Map<ProviderDto>(provider);
			return Ok(mapProvider);
		}

		[HttpPut("Profile/{ProviderId}")]
		public async Task<IActionResult> UpdateProfile(string ProviderId, [FromBody] ProviderDto ProviderUpdate)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (ProviderUpdate == null)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_providerRepository.ProviderExist(ProviderId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var mapProvider = _mapper.Map<Provider>(ProviderUpdate);
			mapProvider.Id = ProviderId;
			var result = await _providerRepository.UpdateProvider(mapProvider);
			if (!result.Succeeded)
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessUpdated);
		}


		[HttpDelete("{ProviderId}")]
		public async Task<IActionResult> DeleteProvider(string ProviderId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ApiResponse.NotValid);
			}
			if (!_providerRepository.ProviderExist(ProviderId))
			{
				return NotFound(ApiResponse.UserNotFound);
			}
			var result = await _providerRepository.DeleteProvider(ProviderId);
			if (!result.Succeeded)
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
			return Ok(ApiResponse.SuccessDeleted);
		}
	}
}
