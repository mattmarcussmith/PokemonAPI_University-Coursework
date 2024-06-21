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
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly ILogger<IReviewRepository> _logger;
        private readonly IMapper _mapper;
        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, ILogger<IReviewRepository> logger ,IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviews()
        {
           try
            {
                _logger.LogInformation("GetReviews was called");
                var reviews = await _reviewRepository.GetReviews();
                if (reviews == null)
                {
                    _logger.LogWarning("No reviews found");
                    return NotFound();
                }
                _logger.LogInformation("Returning reviews");
                var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(reviewsDto);
            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetReviews action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            try
            {
                _logger.LogInformation("GetReviewById was called");
                var review = await _reviewRepository.GetReviewById(reviewId);
                if (review == null)
                {
                    _logger.LogWarning("No review found");
                    return NotFound();
                }
                _logger.LogInformation("Returning review");
                var reviewDto = _mapper.Map<ReviewDto>(review);
                return Ok(reviewDto);
            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetReviewById action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{pokemonId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewsOfAPokemon(int pokemonId)
        {
            try
            {
                _logger.LogInformation("GetReviewsOfAPokemon was called");
                var reviews = await _reviewRepository.GetReviewsOfAPokemonById(pokemonId);
                if (reviews == null)
                {
                    _logger.LogWarning("No reviews found");
                    return NotFound();
                }
                _logger.LogInformation("Returning reviews");
                var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(reviewsDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetReviewsOfAPokemon action");
                return StatusCode(500, "Internal server error");
            }
        }



        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateReview([FromQuery] int reviewerId, [FromQuery] int pokemonId, [FromBody] ReviewDto reviewCreateDto)
        {
            try
            {
                if (reviewCreateDto == null)
                {
                    return BadRequest(ModelState);
                }
                if (!await _reviewerRepository.ReviewerExist(reviewerId))
                {
                    return NotFound();
                }
                if (!await _pokemonRepository.PokemonExist(pokemonId))
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _reviewRepository.ReviewExist(reviewCreateDto.Id))
                {
                    ModelState.AddModelError("", "Review already exists");
                    return StatusCode(404, "Review already exists");
                }
               var reviewMapper = _mapper.Map<Review>(reviewCreateDto);

                if (!await _reviewRepository.CreateReview(reviewMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the review");
                    return StatusCode(500, ModelState);
                }
                return Ok("Review created");

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside CreateReview action");
                return StatusCode(500, "Internal server error");
            }

        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateReviewById(int reviewId, [FromBody] ReviewDto updatedReviewDto)
        {
           try
            {
                if(reviewId != updatedReviewDto.Id)
                {
                    return BadRequest(ModelState);
                }
                
                if(updatedReviewDto == null)
                {
                    _logger.LogWarning("Review object sent from client is null");
                    return BadRequest(ModelState);
                }
            
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }

               var reviewMapper =  _mapper.Map<Review>(updatedReviewDto);
                if (!await _reviewRepository.UpdateReview(reviewMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while updating");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside UpdateReviewById action");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
             
            try
            {
                if (!await _reviewRepository.ReviewExist(reviewId))
                {

                    return NotFound();

                }
                var reviewDelete = await _reviewRepository.GetReviewById(reviewId);

           
                if (!await _reviewRepository.DeleteReview(reviewDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the review");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception)
            {
                _logger.LogError("Something went wrong inside DeleteReviewerById action");
                return StatusCode(500, "Internal server error");
            }

            }
    }
}
