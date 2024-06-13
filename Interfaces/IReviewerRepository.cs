using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IReviewerRepository
    {
        ICollection<Reviewer> GetReviewers();
        Reviewer GetReviewerById(int reviewerId);
        ICollection<Review> GetReviewsByReviewer(int reviewerId);
        bool ReviewerExists(int reviewId);

      
        bool CreateReviewer(Reviewer reviewer);
        bool UpdateReviewerById(Reviewer reviewer);
   
        bool Save();

    }
}
