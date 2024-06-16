using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IOwnerRepository
    {
        Task<ICollection<Owner>> GetOwners();
        Task<Owner> GetOwnerById(int ownerId);
        Task<ICollection<Pokemon>> GetPokemonsByOwner(int ownerId);
        Task<bool> OwnerExists(int ownerId); 
        Task<bool> CreateOwner(Owner owner);
        Task<bool> UpdateOwnerById(Owner owner);
        Task<bool> DeleteOwnerById(Owner owner);
        Task<bool> Save();

    }
}
