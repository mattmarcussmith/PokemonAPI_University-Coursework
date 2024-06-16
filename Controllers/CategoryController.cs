using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;



namespace PokemonReviewer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger, IMapper mapper)
        {

            _categoryRepository = categoryRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                _logger.LogInformation("GetCategories was called");
                var categories = await _categoryRepository.GetCategories();

                if (categories == null)
                {
                    _logger.LogWarning("No categories found");
                    return NotFound();
                }

                _logger.LogInformation("Returning categories");
                var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
                return Ok("Sucessfully created");
            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong inside GetCategories action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult GetCategoryById(int categoryId)
        {
            try
            {
                _logger.LogInformation("GetCategoryById was called");
                var existingCategory = _categoryRepository.GetCategoryById(categoryId);

                if (existingCategory == null)
                {
                    _logger.LogWarning("No categories found");
                    return NotFound();
                }
                _logger.LogInformation("Returning category");
                var categoryDto = _mapper.Map<CategoryDto>(existingCategory);
                return Ok(categoryDto);
            }
            catch
            {
                _logger.LogError("Something went wrong inside GetCategoryById action");
                return StatusCode(500, "Internal server error");

            }
        }


        [HttpGet("{categoryId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPokemonsByCategoryId(int categoryId)
        {
            try
            {
                _logger.LogInformation("GetPokemonsByCategoryId was called");
                var pokemons = await _categoryRepository.GetPokemonsByCategoryId(categoryId);

                if (pokemons == null)
                {
                    _logger.LogWarning("No pokemons found");
                    return NotFound();
                }

                _logger.LogInformation("Returning pokemons");
                var pokemonsDto = _mapper.Map<List<PokemonDto>>(pokemons);
                return Ok(pokemonsDto);
            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong inside GetPokemonsByCategoryId action");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            try
            {
                _logger.LogInformation("CreateCategory was called");
                var category = _mapper.Map<Category>(categoryCreate);
                if (category == null)
                {
                    _logger.LogWarning("Category object sent from client is null");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CategoryDto object");
                    return BadRequest(ModelState);
                }
                var categoryCreateResult = await _categoryRepository.CreateCategory(category);


                return Ok("Content created");

            }
            catch(Exception)
            {
                _logger.LogError("Something went wrong with creating a category");
                return StatusCode(500, "Internal Service error");
                
            }
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCategoryById(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            try
            {
                _logger.LogInformation("UpdateCategoryById was called");
                
                var existingCategory = await _categoryRepository.GetCategoryById(categoryId);

                if (existingCategory == null)
                {
                    _logger.LogWarning("Category object sent from client is null");
                    return BadRequest();
                }
                if (!await _categoryRepository.CategoryExists(categoryId))
                {
                    _logger.LogWarning("Category with that id was not found");
                    return NotFound();
                }
                _mapper.Map(updatedCategory, existingCategory);
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CategoryDto object");
                    return BadRequest(ModelState);
                }
             
                if(!await _categoryRepository.UpdateCategoryById(existingCategory))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
           
                return NoContent();

            } catch(Exception)
            {
                _logger.LogError("Something went wrong with updating a category");
                return StatusCode(500, "Internal Service error");
            }

        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCategoryById(int categoryId)
        {
            try
            {
                _logger.LogInformation("DeleteCategoryById was called");
                var existingCategory = await _categoryRepository.GetCategoryById(categoryId);
                if (existingCategory == null)
                {
                    _logger.LogWarning("Category with that id was not found");
                    return NotFound();
                }
                if(!await _categoryRepository.DeleteCategoryById(existingCategory))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting");
                    return StatusCode(500, ModelState);
                }
                return NoContent();
            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong with deleting a category");
                return StatusCode(500, "Internal Service error");
            }

        }

    }
}
