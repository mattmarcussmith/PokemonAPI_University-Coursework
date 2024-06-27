using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    /// <summary>
    /// Review Repository
    /// </summary>
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ReviewRepository> _logger;
        /// <summary>
        /// Review Repository Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="logger"></param>
        public ReviewRepository(DataContext dataContext, ILogger<ReviewRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        /// <summary>
        /// Review Exist in the database
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        public async Task<bool> ReviewExist(int reviewId)
        {
            try
            {
                return await _dataContext.Reviews.AnyAsync(r => r.Id == reviewId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to check if review with id {reviewId} exists: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Get review by id in the database
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        public async Task<Review> GetReviewById(int reviewId)
        {
            try
            {
                return await _dataContext.Reviews
                               .Where(r => r.Id == reviewId)
                               .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch review with id {reviewId}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Get all reviews from the database
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<Review>> GetReviews()
        {
            try
            {
                return await _dataContext.Reviews
                                  .OrderBy(r => r.Id)
                                  .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch reviews: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Get reviews by pokemon id from the database
        /// </summary>
        /// <param name="pokemonId"></param>
        /// <returns></returns>
        public async Task<ICollection<Review>> GetReviewsOfAPokemonById(int pokemonId)
        {
            try
            {
                return await _dataContext.Reviews
                                       .Where(r => r.Pokemon.Id == pokemonId)
                                       .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch reviews of pokemon with id {pokemonId}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Create review in the database
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        public async Task<bool> CreateReview(Review review)
        {
            try
            {
                await _dataContext.Reviews.AddAsync(review);

                return await Save();

            } catch(Exception ex)
            {
                _logger.LogError($"Failed to create review: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Update review in the database
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        public async Task<bool> UpdateReview(Review review)
        {
            try
            {
                 
                _dataContext.Update(review);
                return await Save();
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to update review with id {review.Id}: {ex.Message}");
                return false;
            }
          
        }
        /// <summary>
        /// Delete reviews from the database
        /// </summary>
        /// <param name="reviews"></param>
        /// <returns></returns>
        public async Task<bool> DeleteReviews(List<Review> reviews)
        {
            try
            {
              _dataContext.RemoveRange(reviews);
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to delete reviews: {ex.Message}");
                return false;
            }
            return await Save();
        }
        /// <summary>
        /// Delete review from the database
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        public async Task<bool> DeleteReview(Review review)
        {
            try
            {
              
                _dataContext.Remove(review);
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to delete review with id {review.Id}: {ex.Message}");
                return false;
            }
            return await Save();
        }
        /// <summary>
        /// Save changes to the database
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0 ? true : false;
        }
    }
}
