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
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PokemonController> _logger;
        public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository, IMapper mapper, ILogger<PokemonController> logger)
        {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPokemons()
        {    
           try
            {
                _logger.LogInformation("GetPokemons was called");
                var pokemons = await _pokemonRepository.GetPokemons();

                if (pokemons == null)
                {
                    _logger.LogWarning("No pokemons found");
                    return NotFound();
                }
                _logger.LogInformation("Returning pokemons");
                var pokemonsDto = _mapper.Map<List<PokemonDto>>(pokemons);
                return Ok(pokemonsDto);
            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetPokemons action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{pokemonId}")]
        [ProducesResponseType(200, Type=typeof(Pokemon))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPokemonById(int pokemonId)
        {
            try
            {
                var existingPokemon = await _pokemonRepository.GetPokemonById(pokemonId);
                if (existingPokemon == null)
                {
                    _logger.LogWarning($"Pokemon with id {pokemonId} not found");
                    return NotFound();
                }
                var pokemon = _mapper.Map<PokemonDto>(existingPokemon);
                return Ok(pokemon);
            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetPokemonById action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{pokemonId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPokemonRating(int pokemonId)
        {
            try
            {
                var existingRating = await _pokemonRepository.GetPokemonRating(pokemonId);
                if (!await _pokemonRepository.PokemonExist(pokemonId))
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(existingRating);
            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetPokemonRating action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreatePokemon([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto pokemonCreateDto)
        {
            try
            {
                _logger.LogInformation("CreatePokemon was called");
                if (pokemonCreateDto == null)
                {
                    _logger.LogWarning("Pokemon object sent from client is null");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the PokemonDto object");
                    return BadRequest(ModelState);
                }
                var pokemonMapper = _mapper.Map<Pokemon>(pokemonCreateDto);
                if (! await _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the pokemon");
                    return StatusCode(500, ModelState);
                }
                return Ok("Pokemon created");

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside CreatePokemon action");
                return StatusCode(500, "Internal server error");
            }
      
         
        }

        [HttpPut("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdatePokemonById(int pokemonId, [FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDto updatedPokemonDto)
        {
           try
            {
                _logger.LogInformation("UpdatePokemonById was called");
           
                if(pokemonId != updatedPokemonDto.Id)
                {
                    return BadRequest(ModelState);
                }
                if(updatedPokemonDto == null)
                {
                    _logger.LogWarning("Pokemon object sent from client is null");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the PokemonDto object");
                    return BadRequest(ModelState);
                }
            
              var pokemonMapper =  _mapper.Map<Pokemon>(updatedPokemonDto);

                if (!await _pokemonRepository.UpdatePokemon(ownerId, categoryId, pokemonMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside UpdatePokemonById action");
                return StatusCode(500, "Internal server error");
            }
              

        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]

        public async Task<IActionResult> DeletePokemon(int pokemonId)
        {
            try
            {

                if (!await _pokemonRepository.PokemonExist(pokemonId))
                {

                    return NotFound();
                }
                var pokemonDelete = await _pokemonRepository.GetPokemonById(pokemonId);

                if (!await _pokemonRepository.DeletePokemon(pokemonDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the reviewer");
                    return StatusCode(500, ModelState);
                }

            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong inside DeletePokemon action");
                return StatusCode(500, "Internal server error");
            }
            return NoContent();
        }
    }
}
