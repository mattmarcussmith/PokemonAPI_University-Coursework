using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewer.Dto;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;
using PokemonReviewer.Repository;

namespace PokemonReviewer.Controllers
{
    /// <summary>
    /// Reviewer Controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly ILogger<ReviewerRepository> _logger;
        private readonly IMapper _mapper;

        /// <summary>
        /// IReviewer for CRUD operations
        /// Logger for debugging
        /// Map ReviewerDto to review and vice versa
        /// </summary>
        /// <param name="reviewerRepository"></param>
        /// <param name="logger"></param>
        /// <param name="mapper"></param>
        public ReviewerController(IReviewerRepository reviewerRepository, ILogger<ReviewerRepository> logger, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get all reviewers
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewers()
        {
            try
            {
                var reviewers = await _reviewerRepository.GetReviewers();
                if (reviewers == null)
                {
                    _logger.LogWarning("No reviewers found");
                    return NotFound();
                }
                var reviewersDto = _mapper.Map<List<ReviewerDto>>(reviewers);
                return Ok(reviewersDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong inside GetReviewers action");
                return StatusCode(500, "Internal server error");
            }
        }
        /// <summary>
        /// Get reviewer by ID
        /// </summary>
        /// <param name="reviewerId"></param>
        /// <returns></returns>
        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewerById(int reviewerId)
        {
            try
            {
                var reviewer = await _reviewerRepository.GetReviewerById(reviewerId);
                if (reviewer == null)
                {
                    _logger.LogError("Reviewer with ID {reviewerId} not found", reviewerId);

                    return NotFound();
                }
                var reviewerDto = _mapper.Map<ReviewerDto>(reviewer);

                return Ok(reviewerDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetReviewsById action for reviewer ID {reviewerId}", reviewerId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Get reviews by reviewer
        /// </summary>
        /// <param name="reviewerId"></param>
        /// <returns></returns>

        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviewsByReviewer(int reviewerId)
        {
            try
            {
                var reviews = await _reviewerRepository.GetReviewsByReviewer(reviewerId);

                if (reviews == null)
                {
                    _logger.LogError("Reviewer with ID {Id} not found", reviewerId);
                    return NotFound();
                }

                if (!await _reviewerRepository.ReviewerExist(reviewerId))
                {
                    ModelState.AddModelError("", "Reviewer with ID {reviewerId} does not exists");
                    return StatusCode(404, ModelState);
                }
                var reviewsDto = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(reviewsDto);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside GetReviewsByReviewer action for reviewer ID {reviewerId}", reviewerId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Create a reviewer
        /// </summary>
        /// <param name="reviewerCreate"></param>
        /// <returns></returns>

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            try
            {

                if (reviewerCreate == null)
                {
                    _logger.LogError("Reviewer with ID {reviewerCreate.Id} not found", reviewerCreate.Id);
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid reviewer object sent from client");
                    return BadRequest(ModelState);
                }

                if (await _reviewerRepository.ReviewerExist(reviewerCreate.Id))
                {
                    ModelState.AddModelError("", "Reviewer already exists");
                    return StatusCode(404, ModelState);
                }

                var reviewerEntity = _mapper.Map<Reviewer>(reviewerCreate);

                if (!await _reviewerRepository.CreateReviewer(reviewerEntity))
                {
                    ModelState.AddModelError("", "Something went wrong while saving the reviewer");
                    return StatusCode(500, ModelState);
                }

                return Ok("Reviewer Created");


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside CreateReviewer action for reviewer {FirstName}", reviewerCreate.FirstName);
                return StatusCode(500, "Internal server error");
            }

        }

        /// <summary>
        /// Update reviewer
        /// </summary>
        /// <param name="reviewerId"></param>
        /// <param name="updatedReviewerDto"></param>
        /// <returns></returns>
        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updatedReviewerDto)
        {
            try
            {
                if (reviewerId != updatedReviewerDto.Id)
                {
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid reviewer object sent from client");
                    return BadRequest(ModelState);
                }

                var reviewerMapper = _mapper.Map<Reviewer>(updatedReviewerDto);
                if (!await _reviewerRepository.UpdateReviewer(reviewerMapper))
                {
                    ModelState.AddModelError("", "Something went wrong while updating the reviewer");
                    return StatusCode(500, ModelState);
                }
                return NoContent();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside updateReviewerDto action for reviewer {FirstName}", updatedReviewerDto.FirstName);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Delete reviewer
        /// </summary>
        /// <param name="reviewerId"></param>
        /// <returns></returns>
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteReviewer(int reviewerId)
        {
            try
            {
                if (!await _reviewerRepository.ReviewerExist(reviewerId))
                {

                    return NotFound();
                }
                var reviewerDelete = await _reviewerRepository.GetReviewerById(reviewerId);
                if (!await _reviewerRepository.DeleteReviewer(reviewerDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the reviewer");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred inside DeleteReviewer action for reviewer {reviewerId}", reviewerId);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
