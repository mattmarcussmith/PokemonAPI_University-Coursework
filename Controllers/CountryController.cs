using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : Controller
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
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
            if(!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }

            var countryById = _mapper.Map<CountryDto>(_countryRepository.GetCountryById(countryId));
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(countryById);
        }

        [HttpGet("{ownerId}/country")]
        [ProducesResponseType(200,  Type = typeof(Country))]
        [ProducesResponseType(400)]

        public IActionResult GetCountryByOwner(int ownerId)
        {
            if(!_countryRepository.CountryExist(ownerId))
            {
                return NotFound();
            }
            var countryByOwner = _mapper.Map<CountryDto>(_countryRepository.GetCountryByOwner(ownerId));
            if(!ModelState.IsValid)
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
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(ownerByCountry);
        }

    }
}
