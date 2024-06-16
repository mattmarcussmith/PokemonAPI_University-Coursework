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
                _logger.LogInformation("GetCountries was called");
                var countries = await _countryRepository.GetCountries();

                if (countries == null)
                {
                    _logger.LogWarning("No countries found");
                    return NotFound();
                }

                _logger.LogInformation("Returning countries");
                var countriesDto = _mapper.Map<List<CountryDto>>(countries);
                return Ok(countriesDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetCountries action");
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
                _logger.LogInformation("GetCountryById was called");
                var country = await _countryRepository.GetCountryById(countryId);

                if (country == null)
                {
                    _logger.LogWarning("Country not found");
                    return NotFound();
                }

                _logger.LogInformation("Returning country");
                var countryDto = _mapper.Map<CountryDto>(country);
                return Ok(countryDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetCountryById action");
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
                _logger.LogInformation("GetCountryByOwner was called");
                var country = await _countryRepository.GetCountryByOwner(ownerId);

                if (country == null)
                {
                    _logger.LogWarning("Country not found");
                    return NotFound();
                }

                _logger.LogInformation("Returning country");
                var countryDto = _mapper.Map<CountryDto>(country);
                return Ok(countryDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetCountryByOwner action");
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
                _logger.LogInformation("GetOwnersByCountry was called");
                var existingOwners = await _countryRepository.GetOwnersByCountryId(countryId);

                if (existingOwners == null)
                {
                    _logger.LogWarning("No owners found");
                    return NotFound();
                }

                _logger.LogInformation("Returning owners");
                var ownersDto = _mapper.Map<List<OwnerDto>>(existingOwners);
                return Ok(ownersDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetOwnersByCountry action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCountry([FromBody] CountryDto countryCreate)
        {
            try
            {
                _logger.LogInformation("CreateCountry was called");
                var country = _mapper.Map<Country>(countryCreate);

                if (country == null)
                {
                    _logger.LogWarning("Country object sent from client is null");
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CountryDto object");
                    return BadRequest(ModelState);
                }

                if (!await _countryRepository.CreateCountry(country))
                {
                    ModelState.AddModelError("", "Something went wrong while saving");
                    return StatusCode(500, ModelState);
                }
                return Ok("Country was created");

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside CreateCountry action");
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

                var existingCountry = await _countryRepository.GetCountryById(countryId);
                

                if (existingCountry == null)
                {
                    _logger.LogWarning("Country object sent from client is null");
                    return BadRequest();
                }
                _mapper.Map(updatedCountryDto, existingCountry);

                if (!await _countryRepository.CountryExist(countryId))
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CountryDto object");
                    return BadRequest(ModelState);
                }

                if (!await _countryRepository.UpdateCountryById(existingCountry))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch (Exception)
            {
                _logger.LogError("Something went wrong inside UpdateCountryById action");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> DeleteCountryById(int countryId)
        {
            try
            {
                _logger.LogInformation("DeleteCountryById was called");
                var country = await _countryRepository.GetCountryById(countryId);

                if (country == null)
                {
                    return NotFound();
                }

                if (!await _countryRepository.DeleteCountryById(country))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside DeleteCountryById action");
                return StatusCode(500, "Internal server error");
            }
            
        }
    }
}
