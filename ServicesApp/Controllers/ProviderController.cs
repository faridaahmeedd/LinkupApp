using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
		[ProducesResponseType(200, Type = typeof(Provider))]
		public IActionResult GetProviders()
		{
			var providers = _providerRepository.GetProviders();
			var mapProviders = _mapper.Map<List<ProviderDto>>(providers);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(mapProviders);
		}


		[HttpGet("{ProviderId}", Name = "GetProviderById")]
		[ProducesResponseType(200, Type = typeof(Provider))]
		public IActionResult GetProvider(string ProviderId)
		{
			if (!_providerRepository.ProviderExist(ProviderId))
			{
				return NotFound();
			}
			var provider = _providerRepository.GetProvider(ProviderId);
			var mapProvider = _mapper.Map<ProviderDto>(provider);
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			return Ok(mapProvider);
		}

		[HttpPost("Profile")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> UpdateProfile(ProviderDto ProviderUpdate, string ProviderId)
		{
			if (ProviderUpdate == null)
			{
				return BadRequest(ModelState);
			}
			if (!_providerRepository.ProviderExist(ProviderId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var mapProvider = _mapper.Map<Provider>(ProviderUpdate);
			mapProvider.Id = ProviderId;
			var result = await _providerRepository.UpdateProvider(mapProvider);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, result.Errors);
			}
			return Ok("Successfully updated");
		}


		[HttpDelete("{ProviderId}")]
		[ProducesResponseType(204)]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteProvider(string ProviderId)
		{
			if (!_providerRepository.ProviderExist(ProviderId))
			{
				return NotFound();
			}
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var result = await _providerRepository.DeleteProvider(ProviderId);
			if (!result.Succeeded)
			{
				ModelState.AddModelError("", "Something went wrong.");
				return StatusCode(500, result.Errors);
			}
			return Ok("Successfully deleted");
		}
	}
}
