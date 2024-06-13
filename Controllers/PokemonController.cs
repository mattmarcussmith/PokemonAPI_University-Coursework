using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;
using PokemonReviewer.Repository;
using System.Reflection.Metadata.Ecma335;
namespace PokemonReviewer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReviewRepository _reviewRepository;

        private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());
            if(!ModelState.IsValid) { return BadRequest(ModelState); }
            return Ok(pokemons);
        }

        [HttpGet("{pokemonId}")]
        [ProducesResponseType(200, Type=typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonById(int pokemonId)
        {
            if(!_pokemonRepository.PokemonExists(pokemonId))
            {
                return NotFound();
            }
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemonById(pokemonId));
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("{pokemonId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId))
            {
                return NotFound();
            }
            var rating = _pokemonRepository.GetPokemonRating(pokemonId);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(rating);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreate)
        {
            if(pokemonCreate == null)
            {
                return BadRequest(ModelState);

            }
            var pokemons = _pokemonRepository.GetPokemons()
                                             .Where(p => p.Name.Trim().ToUpper() == 
                                             pokemonCreate.Name.Trim().ToUpper()).FirstOrDefault();

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
                
            }
            if (pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon already exists");
                return StatusCode(422, ModelState);
            }
            var pokemonMapper = _mapper.Map<Pokemon>(pokemonCreate);

            if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMapper))
            {
                ModelState.AddModelError("", "Something went wrong while saving");

                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");

        }
        [HttpPut("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult UpdatePokemonById(int pokemonId, [FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
            {
                return BadRequest();
            }
            if (pokemonId != updatedPokemon.Id)
            {
                return BadRequest(ModelState);

            }
            if (!_pokemonRepository.PokemonExists(pokemonId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            var pokemonMapper = _mapper.Map<Pokemon>(updatedPokemon);
            

            if (!_pokemonRepository.UpdatePokemonById(ownerId, categoryId, pokemonMapper))
            {
                ModelState.AddModelError("", "Update error on save");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult DeletePokemonById(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId))
            {
                return NotFound();
            }

            var pokemonDelete = _pokemonRepository.GetPokemonById(pokemonId);
            var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokemonId);


            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if(!_reviewRepository.DeleteReviewsById(reviewsToDelete))
            {

            }

            if (!_pokemonRepository.DeletePokemonById(pokemonDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
