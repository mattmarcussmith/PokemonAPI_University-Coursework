using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;
using PokemonReviewer.Repository;

namespace PokemonReviewer.Controllers
{

    /// <summary>
    /// Review Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly ILogger<IReviewRepository> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// IReviewRepository, IReviewerRespository for CRUD operations
        /// Logger for debugging
        /// Map ReviewDto to Review and vice versa
        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, ILogger<IReviewRepository> logger ,IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _logger = logger;
            _mapper = mapper;
        }
        /// <summary>
        /// Get all reviews 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviews()
        {
           try
            {
                var reviews = await _reviewRepository.GetReviews();
                if (reviews == null)
                {
                    _logger.LogWarning("No reviews found");
                    return NotFound();
                }
                var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);

                return Ok(reviewsDto);
            } catch(Exception ex)
            {
                _logger.LogError(ex, "Something went wrong inside GetReviews action");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get review by ID
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewById(int reviewId)
        {
            try
            { 
                var review = await _reviewRepository.GetReviewById(reviewId);
                if (review == null)
                {
                    _logger.LogError("Review with ID {reviewId} not found", reviewId);
                    return NotFound();
                }
                var reviewDto = _mapper.Map<ReviewDto>(review);
                return Ok(reviewDto);

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetReviewsById action for reviewer ID {reviewId}", reviewId);
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Get all reviews of a pokemon
        /// </summary>
        /// <param name="pokemonId"></param>
        /// <returns></returns>

        [HttpGet("{pokemonId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewsOfAPokemon(int pokemonId)
        {
            try
            {
                var reviews = await _reviewRepository.GetReviewsOfAPokemonById(pokemonId);
                if (reviews == null)
                {
                    _logger.LogError("Pokemon with ID {pokemonId} not found", pokemonId);
                    return NotFound();
                }
                var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(reviewsDto);

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetReviewsOfAPokemonById action for {pokemonId}", pokemonId);
                return StatusCode(500, "Internal server error");
            }
        }


        /// <summary>
        /// Create a review
        /// </summary>
        /// <param name="reviewerId"></param>
        /// <param name="pokemonId"></param>
        /// <param name="reviewCreateDto"></param>
        /// <returns></returns>

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
                    _logger.LogError("Review with ID {reviewCreateDto.Id} not found", reviewCreateDto.Id);
                    return NotFound();
                }

                if (!await _reviewerRepository.ReviewerExist(reviewerId))
                {
                    _logger.LogError("Reviewer with ID {reviewerId} not found", reviewerId);
                    return NotFound();
                }
                if (!await _pokemonRepository.PokemonExist(pokemonId))
                {
                    _logger.LogError("Pokemon with ID {pokemonId} not found", pokemonId);
                    return NotFound();
                }
                if(await _reviewRepository.ReviewExist(reviewCreateDto.Id))
                {
                    ModelState.AddModelError("", "Review with already exists");
                    return StatusCode(404, ModelState);
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _reviewRepository.ReviewExist(reviewCreateDto.Id))
                {
                    ModelState.AddModelError("", "Review with ID {reviewCreateDto.Id} already exists");
                    return StatusCode(404, ModelState);
                }
               var reviewMapper = _mapper.Map<Review>(reviewCreateDto);

                if (!await _reviewRepository.CreateReview(reviewMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the review");
                    return StatusCode(500, ModelState);
                }
                return Ok("Review created");

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside CreateReviewer action for reviewer {Id}", reviewCreateDto.Id);
                return StatusCode(500, "Internal server error");
            }

        }

        /// <summary>
        /// Update a review by ID
        /// </summary>
        /// <param name="reviewId"></param>
        /// <param name="updatedReviewDto"></param>
        /// <returns></returns>

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

                if (updatedReviewDto == null)
                {
                    _logger.LogError("Reviewer with ID {updatedReview.Id} not found", updatedReviewDto.Id);
                    return NotFound();
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

            } catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside UpdateReviewerById action for reviewer with ID {Id}", updatedReviewDto.Id);
                return StatusCode(500, "Internal server error");
            }

        }
        /// <summary>
        /// Delete a review by ID
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside DeleteReview action for review {reviewId}", reviewId);
                return StatusCode(500, "Internal server error");
            }

            }
    }
}
