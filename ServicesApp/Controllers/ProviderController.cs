using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.APIs;
using ServicesApp.Dto.User;
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
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				var providers = _providerRepository.GetProviders();
				var mapProviders = _mapper.Map<List<GetProviderDto>>(providers);
				return Ok(mapProviders);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}


		[HttpGet("{ProviderId}", Name = "GetProviderById")]
		public IActionResult GetProvider(string ProviderId)
		{
			try
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
				var mapProvider = _mapper.Map<GetProviderDto>(provider);
				return Ok(mapProvider);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}

		[HttpPut("Profile/{ProviderId}")]
		public async Task<IActionResult> UpdateProfile(string ProviderId, [FromBody] PostProviderDto ProviderUpdate)
		{
			try
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
				await _providerRepository.UpdateProvider(mapProvider);
				return Ok(ApiResponse.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}


		[HttpDelete("{ProviderId}")]
		public async Task<IActionResult> DeleteProvider(string ProviderId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponse.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponse.UserNotFound);
				}
				await _providerRepository.DeleteProvider(ProviderId);
				return Ok(ApiResponse.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponse.SomethingWrong);
			}
		}
	}
}
