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
        private readonly IMapper _mapper;

        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]

        public IActionResult GetReviewers()
        {
          var reviewers = _mapper.Map
                <List<ReviewerDto>>(_reviewerRepository.GetReviewers());
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
           return Ok(reviewers);
        }

        [HttpGet("{reviewerId}")]
        [ProducesResponseType(200, Type = typeof(Reviewer))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewerById(int reviewerId)
        {
            if(!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewerById(reviewerId));
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(reviewer);
        }
        [HttpGet("{reviewerId}/reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Reviewer>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsByReviewer(int reviewerId)
        {
            if(!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));

            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(reviews);
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReviewer([FromBody] ReviewerDto reviewerCreate)
        {
            if (reviewerCreate == null)
            {
                return BadRequest(ModelState);

            }
            var review = _reviewerRepository.GetReviewers()
                                            .Where(r => r.Id == reviewerCreate.Id)
                                            .FirstOrDefault();                        

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            if (review != null)
            {
                ModelState.AddModelError("", "reviewer already exists");
                return StatusCode(422, ModelState);
            }
            var reviewerMapper = _mapper.Map<Reviewer>(reviewerCreate);
           
            if (!_reviewerRepository.CreateReviewer(reviewerMapper))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");

        }


        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult UpdateCategoryById(int reviewerId, [FromBody] ReviewerDto updatedReviewer)
        {
            if (updatedReviewer == null)
            {
                return BadRequest();
            }
            if (reviewerId != updatedReviewer.Id)
            {
                return BadRequest(ModelState);

            }
            if (!_reviewerRepository.ReviewerExists(reviewerId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            var reviewerMapper = _mapper.Map<Reviewer>(updatedReviewer);


            if (!_reviewerRepository.UpdateReviewerById(reviewerMapper))
            {
                ModelState.AddModelError("", "Update error on save");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }
    }
}
