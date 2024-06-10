using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Controllers
{
        [Route("api/[controller]")]
        [ApiController]
    public class OwnerController : Controller
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IMapper _mapper;

        public OwnerController(IOwnerRepository ownerRepository, IMapper mapper) 
        {
            _ownerRepository = ownerRepository;
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
    }
}
