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
        private readonly IMapper _mapper;
        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _reviewerRepository = reviewerRepository;
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            if (!ModelState.IsValid)
            {
                return NotFound();
            }
            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewById(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
            {
                return NotFound();
            }
            var review= _mapper.Map<ReviewDto>(_reviewRepository.GetReviewById(reviewId));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(review);
        }

        [HttpGet("{pokemonId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Review))]
        [ProducesResponseType(400)]
        public IActionResult GetReviewsOfAPokemon(int pokemonId)
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewsOfAPokemon(pokemonId));
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            return Ok(reviews);
        }



        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateReview( [FromQuery] int reviewerId, [FromQuery] int pokemonId, [FromBody] ReviewDto reviewCreate)
        {
            if (reviewCreate == null)
            {
                return BadRequest(ModelState);

            }
            var review = _reviewRepository.GetReviews()
                                             .Where(r => r.Id == reviewCreate.Id).FirstOrDefault();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            if (review != null)
            {
                ModelState.AddModelError("", "review already exists");
                return StatusCode(422, ModelState);
            }
            var reviewMapper = _mapper.Map<Review>(reviewCreate);
            reviewMapper.Reviewer = _reviewerRepository.GetReviewerById(reviewerId);
            reviewMapper.Pokemon = _pokemonRepository.GetPokemonById(pokemonId);
           

            if (!_reviewRepository.CreateReview(reviewMapper))
            {
                ModelState.AddModelError("", "Something went wrong while saving");

                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");

        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]

        public IActionResult UpdateCategoryById(int reviewId, [FromBody] ReviewDto updatedReview)
        {
            if (updatedReview == null)
            {
                return BadRequest();
            }
            if (reviewId != updatedReview.Id)
            {
                return BadRequest(ModelState);

            }
            if (!_reviewerRepository.ReviewerExists(reviewId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                BadRequest(ModelState);
            }

            var reviewMapper = _mapper.Map<Review>(updatedReview);

            if (!_reviewRepository.UpdateReviewById(reviewMapper))
            {
                ModelState.AddModelError("", "Update error on save");
                return StatusCode(500, ModelState);
            }
            return NoContent();

        }
    }
}
