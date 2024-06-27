
using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    /// <summary>
    /// Reviewer Repository
    /// </summary>
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ReviewerRepository> _logger;

        /// <summary>
        /// Reviewer Repository Constructor
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="logger"></param>
        public ReviewerRepository(DataContext dataContext, ILogger<ReviewerRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        /// <summary>
        /// Get reviewer by id in the database
        /// </summary>
        /// <param name="reviewerId"></param>
        /// <returns></returns>
        public async Task<Reviewer> GetReviewerById(int reviewerId)
        {
            try
            {
                return await _dataContext.Reviewers
                              .Where(r => r.Id == reviewerId)
                              .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch reviewer with id {reviewerId}: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Get all reviewers from the database
        /// </summary>
        /// <returns></returns>
        public async Task<ICollection<Reviewer>> GetReviewers()
        {
            try
            {
                return await _dataContext.Reviewers
                                         .OrderBy(r => r.Id)
                                         .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch reviewers: {ex.Message}");
                return null;
            }
        }
        /// <summary>
        /// Get reviews by reviewer from the database
        /// </summary>
        /// <param name="reviewerId"></param>
        /// <returns></returns>
        public async Task<ICollection<Review>> GetReviewsByReviewer(int reviewerId)
        {
            try
            {
                return await _dataContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch reviews by reviewer with id {reviewerId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Check if reviewer exists
        /// </summary>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        public async Task<bool> ReviewerExist(int reviewId)
        {
            try
            {
                return await _dataContext.Reviewers.AnyAsync(r => r.Id == reviewId);
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to check if reviewer with id {reviewId} exists: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Create reviewer in the database
        /// </summary>
        /// <param name="reviewer"></param>
        /// <returns></returns>
        public async Task<bool> CreateReviewer(Reviewer reviewer)
        {
            try
            {
               await _dataContext.AddAsync(reviewer);
                
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to create reviewer: {ex.Message}");
                return false;
            }
            return await Save();
        }
        /// <summary>
        /// Update reviewer in the database
        /// </summary>
        /// <param name="reviewer"></param>
        /// <returns></returns>
        public async Task<bool> UpdateReviewer(Reviewer reviewer)
        {
           try
            {

                _dataContext.Update(reviewer);

            } catch(Exception ex)
            {
                _logger.LogError($"Failed to update reviewer: {ex.Message}");
                return false;
            }
            return await Save();
        }

        /// <summary>
        /// Delete reviewer from the database
        /// </summary>
        /// <param name="reviewer"></param>
        /// <returns></returns>
        public async Task<bool> DeleteReviewer(Reviewer reviewer)
        {
            try
            {
                var reviewerDelete = await _dataContext.Reviewers.Where(r => r.Id == reviewer.Id).FirstOrDefaultAsync();
                var associatedReviewsDelete = await _dataContext.Reviews.Where(r => r.Reviewer.Id == reviewer.Id).ToListAsync();

                if(reviewerDelete != null)
                {
                    _dataContext.RemoveRange(associatedReviewsDelete);
                    _dataContext.Remove(reviewerDelete);
                } 
      
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to delete reviewer {reviewer.Id}: {ex.Message}");
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
