using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;
using PokemonReviewer.Repository;

namespace PokemonReviewer.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OwnerRepository> _logger;

        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper, ILogger<OwnerRepository> logger) 
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
            _logger = logger;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetOwners()
        {
            try
            {
                _logger.LogInformation("GetOwners was called");
                var owners = await _ownerRepository.GetOwners();
                if (owners == null)
                {
                    _logger.LogWarning("No owners found");
                    return NotFound();
                }
                var ownersDto = _mapper.Map<List<OwnerDto>>(owners);
                return Ok(ownersDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetOwners action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetOwnerById(int ownerId)
        {
           try
            {
               var existingOwner = await _ownerRepository.GetOwnerById(ownerId);
                if (existingOwner == null)
                {
                    _logger.LogWarning($"Owner with id {ownerId} not found");
                    return NotFound();
                }
                var owner = _mapper.Map<OwnerDto>(existingOwner);
                return Ok(owner);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetOwnerById action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetPokemonsByOwner(int ownerId)
        {
            try
            {
                var existingPokemons = await _ownerRepository.GetPokemonsByOwnerId(ownerId);
                if (existingPokemons == null)
                {
                    _logger.LogWarning($"No pokemons found for owner with id {ownerId}");
                    return NotFound();
                }
                var pokemonsDto = _mapper.Map<List<PokemonDto>>(existingPokemons);
                return Ok(pokemonsDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetPokemonsByOwner action");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        
        public async Task<IActionResult> CreateOwner([FromBody] OwnerDto ownerCreateDto)
        {
           try
            {
                if (ownerCreateDto == null)
                {
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _ownerRepository.OwnerExist(ownerCreateDto.Id))
                {
                    ModelState.AddModelError("", "Owner already exists");
                    return StatusCode(404, ModelState);
                }

                var ownerMapper = _mapper.Map<Owner>(ownerCreateDto);
                if (!await _ownerRepository.CreateOwner(ownerMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the owner");
                    return StatusCode(500, ModelState);
                }
                return Ok("Owner created");

            } catch (Exception)
            {
                _logger.LogError("Something went wrong inside CreateOwner action");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwnerDto)
        {
            try
            {
                _logger.LogInformation("UpdateOwnerById was called");
                if(ownerId != updatedOwnerDto.Id)
                {
                    return BadRequest(ModelState);
                }
                if (updatedOwnerDto == null)
                {
                    _logger.LogWarning("Owner object sent from client is null");
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the OwnerDto object");
                    return BadRequest(ModelState);
                }


               var ownerMapper = _mapper.Map<Owner>(updatedOwnerDto);
                if (!await _ownerRepository.UpdateOwner(ownerMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside UpdateOwnerById action");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpDelete("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteOwner(int ownerId)
        {
            try
            {
                if (!await _ownerRepository.OwnerExist(ownerId))
                {

                    return NotFound();
                }
                var ownerDelete = await _ownerRepository.GetOwnerById(ownerId);

                if (!await _ownerRepository.DeleteOwner(ownerDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the reviewer");
                    return StatusCode(500, ModelState);
                }

                return NoContent();

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside DeleteOwnerById action");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
