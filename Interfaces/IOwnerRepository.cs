using PokemonReviewer.Models;

namespace PokemonReviewer.Interfaces
{
    public interface IOwnerRepository
    {
        Task<ICollection<Owner>> GetOwners();
        Task<Owner> GetOwnerById(int ownerId);
        Task<ICollection<Pokemon>> GetPokemonsByOwnerId(int ownerId);
        Task<bool> OwnerExist(int ownerId); 
        Task<bool> CreateOwner(Owner owner);
        Task<bool> UpdateOwner(Owner owner);
        Task<bool> DeleteOwner(Owner owner);
        Task<bool> Save();

    }
}
