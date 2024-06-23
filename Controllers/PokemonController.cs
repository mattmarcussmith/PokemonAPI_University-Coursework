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
        private readonly IMapper _mapper;
        private readonly ILogger<PokemonController> _logger;
        public PokemonController(IPokemonRepository pokemonRepository, IReviewRepository reviewRepository, IOwnerRepository ownerRepository,ICategoryRepository categoryRepository, IMapper mapper, ILogger<PokemonController> logger)
        {
            _pokemonRepository = pokemonRepository;
            _ownerRepository = ownerRepository;
            _categoryRepository = categoryRepository;
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
                var pokemons = await _pokemonRepository.GetPokemons();

                if (pokemons == null)
                {
                    _logger.LogWarning("No pokemons found");
                    return NotFound();
                }
                var pokemonsDto = _mapper.Map<List<PokemonDto>>(pokemons);
                return Ok(pokemonsDto);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Something went wrong inside GetReviews action");
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
                    _logger.LogWarning($"Pokemon with ID {pokemonId} not found", pokemonId);
                    return NotFound();
                }
                var pokemon = _mapper.Map<PokemonDto>(existingPokemon);
                return Ok(pokemon);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetPokemonById action for pokemon with ID {pokemonId}", pokemonId);
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
                    _logger.LogWarning($"Pokemon with ID {pokemonId} not found", pokemonId);
                    return NotFound();
                }
              
                return Ok(existingRating);
            } catch(Exception)
            {
               _logger.LogError("An error occurred inside GetPokemonRating action for ID {pokemonId}", pokemonId);
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
               
                if (pokemonCreateDto == null)
                {
                    _logger.LogWarning("Pokemon object sent from client is null");
                    return BadRequest();
                }

                if (!await _ownerRepository.OwnerExist(ownerId))
                {
                    ModelState.AddModelError("", "Owner does not exist");
                    return StatusCode(404, ModelState);
                }

                if (!await _categoryRepository.CategoryExist(categoryId))
                {
                    ModelState.AddModelError("", "Category does not exist");
                    return StatusCode(404, ModelState);
                }
                if(await _pokemonRepository.PokemonExist(pokemonCreateDto.Id))
                {
                    ModelState.AddModelError("", "Pokemon already exists");
                    return StatusCode(422, ModelState);
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

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside CreatePokemon action for {Id}", pokemonCreateDto.Id);
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
           
                if(pokemonId != updatedPokemonDto.Id)
                {
                    return BadRequest(ModelState);
                }
                if(updatedPokemonDto == null)
                {
                    _logger.LogWarning("Pokemon object sent from client is null");
                    return BadRequest();
                }
                if (!await _ownerRepository.OwnerExist(ownerId))
                {
                    ModelState.AddModelError("", "Owner does not exist");
                    return StatusCode(404, ModelState);
                }
                if(!await _categoryRepository.CategoryExist(categoryId))
                {
                    ModelState.AddModelError("", "Category does not exist");
                    return StatusCode(404, ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the PokemonDto object");
                    return BadRequest(ModelState);
                }
            
              var pokemonMapper =  _mapper.Map<Pokemon>(updatedPokemonDto);

                if (!await _pokemonRepository.UpdatePokemon(ownerId, categoryId, pokemonMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while updating the pokemon");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch(Exception ex)
            {
                _logger.LogError(ex, "$Something went wrong inside UpdatePokemonById action with {Id}", updatedPokemonDto.Id);
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
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside DeletePokemon action for review {pokemonId}", pokemonId);
                return StatusCode(500, "Internal server error");
            }
            
        }
    }
}
