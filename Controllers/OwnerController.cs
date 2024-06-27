using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;
using PokemonReviewer.Repository;

namespace PokemonReviewer.Controllers
{
    /// <summary>
    /// Owner Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<OwnerRepository> _logger;

        /// <summary>
        /// IOwnerRepository for CRUD operations
        /// Logger for debugging
        /// Map Owner to OwnerDto
        /// </summary>
        /// <param name="ownerRepository"></param>
        /// <param name="countryRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="logger"></param>
        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper, ILogger<OwnerRepository> logger)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
            _logger = logger;

        }

        /// <summary>
        /// Get all owners
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetOwners()
        {
            try
            {
                var owners = await _ownerRepository.GetOwners();
                if (owners == null)
                {
                    _logger.LogWarning("No owners found");
                    return NotFound();
                }
                var ownersDto = _mapper.Map<List<OwnerDto>>(owners);
                return Ok(ownersDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetOwners action");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get owner by ID
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
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
                    _logger.LogWarning($"Owner with ID {ownerId} not found", ownerId);
                    return NotFound();
                }
                var owner = _mapper.Map<OwnerDto>(existingOwner);
                return Ok(owner);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetOwnerById action for ID {ownerId}", ownerId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get all pokemons by owner ID
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
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
                    _logger.LogWarning($"No pokemons found for owner with ID {ownerId}");
                    return NotFound();
                }
                var pokemonsDto = _mapper.Map<List<PokemonDto>>(existingPokemons);
                return Ok(pokemonsDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetPokemonById action for ID {ownerId}", ownerId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create owner
        /// </summary>
        /// <param name="ownerCreateDto"></param>
        /// <returns></returns>
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

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside CreateOwner action for ID {Id}", ownerCreateDto.Id);
                return StatusCode(500, "Internal server error");
            }

        }

        /// <summary>
        /// Update owner
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="updatedOwnerDto"></param>
        /// <returns></returns>
        [HttpPut("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateOwner(int ownerId, [FromBody] OwnerDto updatedOwnerDto)
        {
            try
            {
                if (ownerId != updatedOwnerDto.Id)
                {
                    return BadRequest(ModelState);
                }
                if (updatedOwnerDto == null)
                {
                    _logger.LogWarning("Owner object sent from client is null");
                    return NotFound();
                }

                if (!await _ownerRepository.OwnerExist(updatedOwnerDto.Id))
                {
                    ModelState.AddModelError("", "Owner does not exist");
                    return StatusCode(404, ModelState);
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

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside UpdateOwner action for ID {Id}", updatedOwnerDto.Id);
                return StatusCode(500, "Internal server error");
            }

        }

        /// <summary>
        /// Delete owner
        /// </summary>
        /// <param name="ownerId"></param>
        /// <returns></returns>
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

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside DeleteOwner action for review {Id}", ownerId);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
