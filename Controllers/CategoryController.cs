using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;
using PokemonReviewer.Repository;



namespace PokemonReviewer.Controllers
{
    /// <summary> 
    ///  Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class CategoryController : Controller
    {
       
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<CategoryController> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// ICategroyRepository for CRUD operations
        /// Logger for debugging
        /// Map categoryDto to category and vice versa
        /// </summary>
        /// <param name="categoryRepository"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public CategoryController(ICategoryRepository categoryRepository, ILogger<CategoryController> logger, IMapper mapper)
        {

            _categoryRepository = categoryRepository;
            _logger = logger;
            _mapper = mapper;
        }


        /// <summary>
        /// Retrieves all categories
        /// </summary>
        /// <remarks>Returns a list of pokemon categories</remarks>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetCategories()
        {
            try
            {
                var categories = await _categoryRepository.GetCategories();

                if (categories == null)
                {
                    _logger.LogWarning("No categories found");
                    return NotFound();
                }

                _logger.LogInformation("Returning categories");
                var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
                return Ok(categoriesDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetCategories action");
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Retrieves a category by ID
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
       
        public async Task<IActionResult> GetCategoryById(int categoryId)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategoryById(categoryId);

                if (existingCategory == null)
                {
                    _logger.LogWarning("No categories found");
                    return NotFound();
                }
                var categoryDto = _mapper.Map<CategoryDto>(existingCategory);
                return Ok(categoryDto);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetCategoryById action with ID {categoryId}", categoryId);
                return StatusCode(500, "Internal server error");
            }
           
        }

        /// <summary>
        /// Get pokemons by category ID
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpGet("{categoryId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetPokemonsByCategoryId(int categoryId)
        {
            try
            {
  
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetPokemonsByCategoryId action for ID {categoryId}", categoryId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="categoryCreate"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            try
            {   
                if (categoryCreate == null)
                {
                    _logger.LogWarning("Category object sent from client is null");
                    return BadRequest();
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CategoryDto object");
                    return BadRequest(ModelState);
                }
                if(await _categoryRepository.CategoryExist(categoryCreate.Id))
                {
                    ModelState.AddModelError("", "Category already exists");
                    return StatusCode(404, ModelState);
                }
                var categoryMapper = _mapper.Map<Category>(categoryCreate);
                

                if (!await _categoryRepository.CreateCategory(categoryMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the category");
                    return StatusCode(500, ModelState);
                }

                return Ok("Category created");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside CreateCategory action for ID {Id}", categoryCreate.Id);
                return StatusCode(500, "Internal Service error");
                
            }
        }

        /// <summary>
        /// Update a category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="updatedCategoryDto"></param>
        /// <returns></returns>

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCategory(int categoryId, [FromBody] CategoryDto updatedCategoryDto)
        {
            try
            { 
       
                if(categoryId != updatedCategoryDto.Id)
                {
                    return BadRequest(ModelState);
                }
                if (updatedCategoryDto == null)
                {
                    _logger.LogWarning("Category object sent from client is null");
                    return BadRequest();
                }
                if (!await _categoryRepository.CategoryExist(categoryId))
                {
                    ModelState.AddModelError("", "Category does not exist");
                    return StatusCode(404, ModelState);
                }
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for the CategoryDto object");
                    return BadRequest(ModelState);
                }

                var categoryMapper = _mapper.Map<Category>(updatedCategoryDto);

                if (!await _categoryRepository.UpdateCategory(categoryMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside UpdateCategory action for ID {Id}", updatedCategoryDto.Id);
                return StatusCode(500, "Internal Service error");
            }
        }

        /// <summary>
        /// Delete a category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            try
            {
                if (!await _categoryRepository.CategoryExist(categoryId))
                {

                    return NotFound();
                }
                var categoryDelete = await _categoryRepository.GetCategoryById(categoryId);

                if (!await _categoryRepository.DeleteCategory(categoryDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the reviewer");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside DeletePokemon action for ID {Id}", categoryId);
                return StatusCode(500, "Internal Service error");
            }

        }

    }
}
