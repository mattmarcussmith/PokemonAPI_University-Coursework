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
        public CountryController(ICountryRepository countryRepository, IOwnerRepository ownerRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _ownerRepository = ownerRepository;
            _mapper = mapper;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]

        public IActionResult GetCountries()
        {
            var country = _mapper.Map<List<CountryDto>>(_countryRepository.GetCountries());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(country);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]

        public IActionResult GetCountryById(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }

            var countryById = _mapper.Map<CountryDto>(_countryRepository.GetCountryById(countryId));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(countryById);
        }

        [HttpGet("{ownerId}/country")]
        [ProducesResponseType(200, Type = typeof(Country))]
        [ProducesResponseType(400)]

        public IActionResult GetCountryByOwner(int ownerId)
        {
            if (!_countryRepository.CountryExist(ownerId))
            {
                return NotFound();
            }
            var countryByOwner = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(countryByOwner);
        }

        [HttpGet("{countryId}/owners")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Country>))]
        [ProducesResponseType(400)]

        public IActionResult GetOwnersByCountry(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }
            var ownerByCountry = _countryRepository.GetOwnersByCountry(countryId);
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(ownerByCountry);
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromBody] CountryDto countryCreate)
        {
            if (countryCreate == null)
            {
                return BadRequest(ModelState);
            }

            var country = _countryRepository.GetCountries()
                                            .Where(c => c.Name.Trim().ToUpper() == countryCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists");
                return StatusCode(422, ModelState);
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var countryMapper = _mapper.Map<Country>(countryCreate);

            if (!_countryRepository.CreateCountry(countryMapper))
            {
                ModelState.AddModelError("", "Something went wrong while saving.");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created");
        }



        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult UpdateCountryById(int countryId, [FromBody] CountryDto updatedCountry)
        {
            if (updatedCountry == null)
            {
                return BadRequest();
            }
            if (countryId != updatedCountry.Id)
            {
                return BadRequest(ModelState);

            }
            if (!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            var countryMapper = _mapper.Map<Country>(updatedCountry);


            if (!_countryRepository.UpdateCountryById(countryMapper))
            {
                ModelState.AddModelError("", "Update error on save");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult DeleteCountryById(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }

            var countryDelete = _countryRepository.GetCountryById(countryId);
        


            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
          

            if (!_countryRepository.DeleteCountryById(countryDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
