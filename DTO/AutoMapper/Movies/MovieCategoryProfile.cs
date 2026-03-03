using AutoMapper;
using DTO.MovieCategories;
using DTO.MusicCategories;
using Entity;

namespace DTO.AutoMapper.Movies;

public class MovieCategoryProfile:Profile
{
    public MovieCategoryProfile()
    {
        CreateMap<CreateMovieCategoryDto, MovieCategory>().ReverseMap();
        CreateMap<UpdateMovieCategoryDto, MovieCategory>().ReverseMap();
        CreateMap<GetMovieCategoryDto, MovieCategory>().ReverseMap();
    }
}