using AutoMapper;
using DTO.MovieCategories;
using DTO.MusicCategories;
using Entity;

namespace DTO.AutoMapper.Movies;

public class MovieCategoryProfile:Profile
{
    public MovieCategoryProfile()
    {
        CreateMap<CreateMovieCategoryDto, MovieCategory>();
        CreateMap<UpdateMovieCategoryDto, MovieCategory>();
        
        // Map MovieCategory to GetMovieCategoryDto, including the SuggestedMusicCategories
        CreateMap<MovieCategory, GetMovieCategoryDto>()
            .ForMember(dest => dest.SuggestedMusicCategories, 
                opt => opt.MapFrom(src => src.SuggestedMusicCategories));
        
        CreateMap<GetMovieCategoryDto, MovieCategory>();
    }
}