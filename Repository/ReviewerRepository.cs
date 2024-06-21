
using AutoMapper.Configuration.Conventions;
using Microsoft.EntityFrameworkCore;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{
    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ReviewerRepository> _logger;

        public ReviewerRepository(DataContext dataContext, ILogger<ReviewerRepository> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
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

        public async Task<bool> UpdateReviewer(Reviewer reviewer)
        {
           try
            {

                var existingReviewer = await _dataContext.Reviewers
                                        .Where(r => r.Id == reviewer.Id)
                                        .FirstOrDefaultAsync();
        
                if (existingReviewer != null)
                {
                    _logger.LogError($"Reviewer with id {reviewer.Id} not found");
                    return false;
                }
                if (existingReviewer == null)
                {
                    _logger.LogError($"Reviewer with id {reviewer.Id} not found");
                    return false;
                }
                _dataContext.Update(existingReviewer);

            } catch(Exception ex)
            {
                _logger.LogError($"Failed to update reviewer: {ex.Message}");
                return false;
            }
            return await Save();
        }

        public async Task<bool> DeleteReviewer(Reviewer reviewer)
        {
            try
            {
                
                _dataContext.Remove(reviewer);         
            } catch(Exception ex)
            {
                _logger.LogError($"Failed to delete reviewer {reviewer.Id}: {ex.Message}");
                return false;
            }
            return await Save();
        }
        public async Task<bool> Save()
        {
            return await _dataContext.SaveChangesAsync() > 0 ? true : false;
        }

    }
}
