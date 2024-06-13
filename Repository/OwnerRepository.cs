using PokemonReviewer.Data;
using PokemonReviewer.Interfaces;
using PokemonReviewer.Models;

namespace PokemonReviewer.Repository
{



    public class OwnerRepository : IOwnerRepository
    {
        public readonly DataContext _dataContext;
        public OwnerRepository(DataContext dataContext) 
        { 
            _dataContext = dataContext;
        
        }
        public bool OwnerExists(int ownerId)
        {
            return _dataContext.Owners.Any(o => o.Id == ownerId);
        }
        public Owner GetOwnerById(int ownerId)
        {
            return _dataContext.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
        }


        public ICollection<Owner> GetOwners()
        {
            return _dataContext.Owners.OrderBy(o => o.Id)
                                      .ToList();
        }

        public ICollection<Pokemon> GetPokemonsByOwner(int ownerId)
        {
            return _dataContext.PokemonOwners.Where(o => o.OwnerId == ownerId)
                                             .Select(p => p.Pokemon)
                                             .ToList();
        }

        public bool CreateOwner(Owner owner)
        {
            _dataContext.Add(owner);
            return Save();
        }
        public bool UpdateOwnerById(Owner owner)
        {
            _dataContext.Update(owner);
            return Save();
        }
        public bool DeleteOwnerById(Owner owner)
        {
            _dataContext.Remove(owner);
            return Save();
        }
        public bool Save()
        {
            return _dataContext.SaveChanges() > 0 ? true : false;
        }

      
    }
}
