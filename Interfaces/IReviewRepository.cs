using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IReviewRepository
    {
        Task<ICollection<Review>> GetReviews();
        Task<Review> GetReviewById(int reviewId);
        Task<ICollection<Review>> GetReviewsOfAPokemonById(int pokemonId);
        Task<bool> ReviewExist(int reviewId);
        Task<bool> CreateReview(Review review);
        Task<bool> UpdateReview(Review review);
        Task<bool> DeleteReview(Review review);
        Task<bool> DeleteReviews(List<Review> reviews);
        Task<bool> Save();
    }
}
