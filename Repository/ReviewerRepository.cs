
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{


    public class ReviewerRepository : IReviewerRepository
    {
        private readonly DataContext _dataContext;
       
        public ReviewerRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

     

        public Reviewer GetReviewerById(int reviewerId)
        {
            return _dataContext.Reviewers
                               .Where(r => r.Id == reviewerId)
                               .FirstOrDefault();
        }

        public ICollection<Reviewer> GetReviewers()
        {
            return _dataContext.Reviewers
                                         .OrderBy(r => r.Id)
                                         .ToList();
        }

        public ICollection<Review> GetReviewsByReviewer(int reviewerId)
        {
            return _dataContext.Reviews.Where(r => r.Reviewer.Id == reviewerId).ToList();
        }

        public bool ReviewerExists(int reviewId)
        {
            return _dataContext.Reviewers.Any(r => r.Id == reviewId);
        }

        public bool CreateReviewer(Reviewer reviewer)
        {
            _dataContext.Add(reviewer);
            return Save();
        }

        public bool UpdateReviewerById(Reviewer reviewer)
        {
            _dataContext.Update(reviewer);
            return Save();
        }

        public bool DeleteReviewerById(Reviewer reviewer)
        {
            
            _dataContext.Remove(reviewer);
            return Save();
        }
        public bool Save()
        {
            return _dataContext.SaveChanges() > 0 ? true : false;
        }

    }
}
