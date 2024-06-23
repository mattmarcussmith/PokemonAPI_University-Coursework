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
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CountryController> _logger;
        public CountryController(ICountryRepository countryRepository, IOwnerRepository ownerRepository, IMapper mapper, ILogger<CountryController> logger)
        {
            _countryRepository = countryRepository;
            _ownerRepository = ownerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCountries()
        {
           try
            {
                var countries = await _countryRepository.GetCountries();

                if (countries == null)
                {
                    _logger.LogWarning("No countries found");
                    return NotFound();
                }

                var countriesDto = _mapper.Map<List<CountryDto>>(countries);
                return Ok(countriesDto);

            } catch(Exception ex)
            {
                _logger.LogError(ex, "Something went wrong inside GetCountries action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCountryById(int countryId)
        {
            try
            {
                var country = await _countryRepository.GetCountryById(countryId);

                if (country == null)
                {
                    _logger.LogWarning("Country not found");
                    return NotFound();
                }

                var countryDto = _mapper.Map<CountryDto>(country);
                return Ok(countryDto);

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetCountryById action for ID {countryId}", countryId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{ownerId}/country")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCountryByOwner(int ownerId)
        {
            try
            {
                var country = await _countryRepository.GetCountryByOwnerId(ownerId);

                if (country == null)
                {
                    _logger.LogWarning("Country not found");
                    return NotFound();
                }
                var countryDto = _mapper.Map<CountryDto>(country);
                return Ok(countryDto);

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetCountryByOwner action for ID {ownerId}", ownerId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{countryId}/owners")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> GetOwnersByCountryId(int countryId)
        {
            try
            {

                var existingOwners = await _countryRepository.GetOwnersByCountryId(countryId);

                if (existingOwners == null)
                {
                    _logger.LogWarning("No owners found");
                    return NotFound();
                }

                var ownersDto = _mapper.Map<List<OwnerDto>>(existingOwners);
                return Ok(ownersDto);

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetOwnersByCountry action for ID {countryId}", countryId);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDto countryCreateDto)
        {
            try
            {
    
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CountryDto object");
                    return BadRequest(ModelState);
                }
                if(await _countryRepository.CountryExist(countryCreateDto.Id))
                {
                    ModelState.AddModelError("", "Country already exists");
                    return StatusCode(404, ModelState);
                }
            
                var countryMapper = _mapper.Map<Country>(countryCreateDto);
   
                if (!await _countryRepository.CreateCountry(countryMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(500, ModelState);
                }
                return Ok("Country created");

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside CreateCountry action for {Id}", countryCreateDto.Id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCountryById(int countryId, [FromBody] CountryDto updatedCountryDto)
        {
           try
            {
                _logger.LogInformation("UpdateCountryById was called");

                if(countryId != updatedCountryDto.Id)
                {
                    return BadRequest(ModelState);
                }
                if (!await _countryRepository.CountryExist(countryId))
                {
                    ModelState.AddModelError("", "Country does not exist");
                    return StatusCode(404, ModelState);
                }
                if (updatedCountryDto == null)
                {
                    _logger.LogWarning("Country object sent from client is null");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CountryDto object");
                    return BadRequest(ModelState);
                }

               var countryMapper =  _mapper.Map<Country>(updatedCountryDto);

                if (!await _countryRepository.UpdateCountry(countryMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside UpdateCountryById action for ID {countryId}", countryId);
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> DeleteCountry(int countryId)
        {
            try
            {
                if (!await _countryRepository.CountryExist(countryId))
                {

                    return NotFound();
                }
                var countryDelete = await _countryRepository.GetCountryById(countryId);

                if (!await _countryRepository.DeleteCountry(countryDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the country");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside DeleteCountry action for ID {countryId}", countryId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
