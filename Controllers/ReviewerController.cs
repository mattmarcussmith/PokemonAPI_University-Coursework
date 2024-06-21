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
    public class ReviewerController : Controller
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly ILogger<ReviewerRepository> _logger;
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, ILogger <ReviewerRepository> logger,  IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewers()
        {
            try
            {
                _logger.LogInformation("GetReviewers was called");
                var reviewers = await _reviewerRepository.GetReviewers();
                if (reviewers == null)
                {
                    _logger.LogWarning("No reviewers found");
                    return NotFound();
                }
                _logger.LogInformation("Returning reviewers");
                var reviewersDto = _mapper.Map<List<ReviewerDto>>(reviewers);
                return Ok(reviewersDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetReviewers action");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetReviewerById(int reviewerId)
        {
           try
            {
                _logger.LogInformation("GetReviewerById was called");
                var reviewer = await _reviewerRepository.GetReviewerById(reviewerId);
                if (reviewer == null)
                {
                    _logger.LogWarning("No reviewer found");
                    return NotFound();
                }
                _logger.LogInformation("Returning reviewer");

                var reviewerDto = _mapper.Map<ReviewerDto>(reviewer);

                return Ok(reviewerDto);

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside GetReviewerById action");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetReviewsByReviewer(int reviewerId)
        {
            try
            {
                _logger.LogInformation("GetReviewsByReviewer was called");
                var reviews = await _reviewerRepository.GetReviewsByReviewer(reviewerId);
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
                _logger.LogError("Something went wrong inside GetReviewsByReviewer action");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async  Task<IActionResult> CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            try
            {
                _logger.LogInformation("CreateReviewer was called");
                

                if (reviewerCreate == null)
                {
                    _logger.LogError("Reviewer object sent from client is null");
                    return BadRequest();
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


            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside CreateReviewer action");
                return StatusCode(500, "Internal server error");
            }

        }


        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<IActionResult> UpdateReviewer(int reviewerId, [FromBody] ReviewerDto updatedReviewerDto)
        {
            try
            {
                _logger.LogInformation("UpdateReviewerById was called");
            
                if(reviewerId != updatedReviewerDto.Id)
                {
                    return BadRequest(ModelState);
                }
                if (updatedReviewerDto == null)
                {
                    _logger.LogWarning("Reviewer object sent from client is null");
                    return NotFound();
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

            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside UpdateReviewerById action");
                return StatusCode(500, "Internal server error");
            }

        }
        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public async Task<IActionResult> DeleteReviewer(int reviewerId)
        {
           try
            {
                if(!await _reviewerRepository.ReviewerExist(reviewerId)) {

                    return NotFound();

                }
                var reviewerDelete = await _reviewerRepository.GetReviewerById(reviewerId);

                if (!await _reviewerRepository.DeleteReviewer(reviewerDelete))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting the reviewer");
                    return StatusCode(500, ModelState);
                }

             return NoContent();
            } catch(Exception)
            {
                _logger.LogError("Something went wrong inside DeleteReviewerById action");
                return StatusCode(500, "Internal server error");
            }
     

        }
    }
}
