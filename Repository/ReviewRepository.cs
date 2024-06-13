using AutoMapper;
using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{

    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _dataContext;

        public ReviewRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
           
        }
        public bool ReviewExists(int reviewId)
        {
            return _dataContext.Reviews.Any(r => r.Id == reviewId);
        }
        public Review GetReviewById(int reviewId)
        {
            return _dataContext.Reviews
                               .Where(r => r.Id == reviewId)
                               .FirstOrDefault();
        }

        public ICollection<Review> GetReviews()
        {
            return _dataContext.Reviews
                               .OrderBy(r => r.Id)
                               .ToList();               
        }

        public ICollection<Review> GetReviewsOfAPokemon(int pokemonId)
        {
            return _dataContext.Reviews.Where(r => r.Pokemon.Id == pokemonId)
                                       .ToList();
        }

        public bool CreateReview(Review review)
        {
            _dataContext.Add(review);
            return Save();
        }
        public bool UpdateReviewById(Review review)
        {
            _dataContext.Update(review);
            return Save();
        }
        public bool Save()
        {
            return _dataContext.SaveChanges() > 0 ? true : false;
        }

     
    }
}
