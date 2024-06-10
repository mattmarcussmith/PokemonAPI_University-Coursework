using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReviewById(int reviewId);
        ICollection<Review> GetReviewsOfAPokemon(int pokemonId);
        bool ReviewExists(int reviewId);
    }
}
