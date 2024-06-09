using AutoMapper;
using PokemonReviewer.Dto;
using PokemonReviewer.Models;

namespace PokemonReviewer.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<Pokemon, PokemonDto>();
            CreateMap<Category, CategoryDto>();


        
        }

    }
}
