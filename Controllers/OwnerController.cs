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

        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper) 
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;

        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]
        public IActionResult GetOwners()
        {
            var owners = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            if(!ModelState.IsValid)
            {
                return NotFound();
            }
            return Ok(owners);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwnerById(int ownerId)
        {
            if(!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            var ownerById = _mapper.Map<OwnerDto>(_ownerRepository.GetOwnerById(ownerId));
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(ownerById);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        [ProducesResponseType(400)]

        public IActionResult GetPokemonsByOwner(int ownerId)
        {
            var owner = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetPokemonsByOwner(ownerId));
            if(!ModelState.IsValid)
            {
                return BadRequest();

            }
            return Ok(owner);

        }
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto ownerCreate)
        {
            if(ownerCreate == null)
            {
                return BadRequest(ModelState);
            }

            var owner = _ownerRepository.GetOwners().Where(o => o.LastName.Trim().ToUpper() == ownerCreate.LastName.ToUpper()).FirstOrDefault();

            if(owner != null)
            {
                ModelState.AddModelError("", "This owner already exist");
                return StatusCode(422, ModelState);
            }
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerMapper = _mapper.Map<Owner>(ownerCreate);
            ownerMapper.Country = _countryRepository.GetCountryById(countryId);

            if(!_ownerRepository.CreateOwner(ownerMapper))
            {
                 ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }
            return Ok("successfully created");

        }

        [HttpPut("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult UpdateCategoryById(int ownerId, [FromBody] OwnerDto updatedOwner)
        {
            if (updatedOwner == null)
            {
                return BadRequest();
            }
            if (ownerId != updatedOwner.Id)
            {
                return BadRequest(ModelState);

            }
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            var ownerMapper = _mapper.Map<Owner>(updatedOwner);


            if (!_ownerRepository.UpdateOwnerById(ownerMapper))
            {
                ModelState.AddModelError("", "Update error on save");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }
        [HttpDelete("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult DeleteCountryById(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            var ownerDelete = _ownerRepository.GetOwnerById(ownerId);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }


            if (!_ownerRepository.DeleteOwnerById(ownerDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
