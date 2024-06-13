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
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository, IMapper mapper) 
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }


        
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategories() 
        {
            var categories = _mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories());
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(categories);
        }

        [HttpGet("{categoryId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Category>))]
        [ProducesResponseType(400)]
        public IActionResult GetCategoryById(int categoryId) 
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var category = _mapper.Map<CategoryDto>(_categoryRepository.GetCategoryById(categoryId));
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(category);
        }

        [HttpGet("{categoryId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonsByCategoryId(int categoryId)
        {
            var pokemons = _mapper.Map
                <List<PokemonDto>>(_categoryRepository.GetPokemonsByCategoryId(categoryId));

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(pokemons);

        }

        
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]

        public IActionResult CreateCategory([FromBody] CategoryDto categoryCreate)
        {
            if(categoryCreate == null)
            {
                return BadRequest();
            }

            var category = _categoryRepository.GetCategories()
                                              .Where(c => c.Id == categoryCreate.Id)
                                              .FirstOrDefault();
            if(category != null)
            {
                ModelState.AddModelError("", "Category already exists");
                return StatusCode(422, ModelState);
            }

            var categoryMapper = _mapper.Map<Category>(categoryCreate);

            if(!_categoryRepository.CreateCategory(categoryMapper))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState); 
            }

            return Ok("Successfully created");
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult UpdateCategoryById(int categoryId, [FromBody] CategoryDto updatedCategory)
        {
            if(updatedCategory == null)
            {
                return BadRequest();
            }
            if(categoryId != updatedCategory.Id)
            {
                return BadRequest(ModelState);

            }
            if(!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }
            if(!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            var categoryMapper = _mapper.Map<Category>(updatedCategory);
    

            if(!_categoryRepository.UpdateCategoryById(categoryMapper))
            {
                ModelState.AddModelError("", "Update error on save");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult DeleteCategoryById(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var categoryDelete = _categoryRepository.GetCategoryById(categoryId);
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!_categoryRepository.DeleteCategoryById(categoryDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
