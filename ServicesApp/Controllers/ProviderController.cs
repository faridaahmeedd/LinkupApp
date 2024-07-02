using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServicesApp.Dto.User;
using ServicesApp.Helper;
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
					return BadRequest(ApiResponses.NotValid);
				}
				var providers = _providerRepository.GetProviders();
				var mapProviders = _mapper.Map<List<GetProviderDto>>(providers);
				return Ok(mapProviders);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}


		[HttpGet("{ProviderId}")]
		public IActionResult GetProvider(string ProviderId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				var provider = _providerRepository.GetProvider(ProviderId);
				var mapProvider = _mapper.Map<GetProviderDto>(provider);
				return Ok(mapProvider);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("Profile/{ProviderId}")]
		public async Task<IActionResult> UpdateProfile(string ProviderId, [FromBody] PostProviderDto ProviderUpdate)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (ProviderUpdate == null)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				var mapProvider = _mapper.Map<Provider>(ProviderUpdate);
				mapProvider.Id = ProviderId;
				await _providerRepository.UpdateProvider(mapProvider);
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}

		[HttpPut("Approve/{ProviderId}")]
		public async Task<IActionResult> ApproveProvider(string ProviderId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				await _providerRepository.ApproveProvider(ProviderId);
				return Ok(ApiResponses.SuccessUpdated);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}


		[HttpDelete("{ProviderId}")]
		public async Task<IActionResult> DeleteProvider(string ProviderId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ApiResponses.NotValid);
				}
				if (!_providerRepository.ProviderExist(ProviderId))
				{
					return NotFound(ApiResponses.UserNotFound);
				}
				await _providerRepository.DeleteProvider(ProviderId);
				return Ok(ApiResponses.SuccessDeleted);
			}
			catch
			{
				return StatusCode(500, ApiResponses.SomethingWrong);
			}
		}
	}
}
